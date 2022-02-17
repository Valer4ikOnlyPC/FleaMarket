﻿using Domain.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly long _fileSizeLimit;
        private string _filePath;
        private enum FileType
        {
            Jpg,
            Png,
            Unknown
        }
        private static readonly Dictionary<FileType, byte[]> _fileSignature =
            new()
            {
                { FileType.Jpg, new byte[] { 0xFF, 0xD8, 0xFF } },
                { FileType.Png, new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
            };

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
            _fileSizeLimit = Int32.Parse(configuration["FileSizeLimit"]);
            _filePath = _configuration["FileDirectory"];
        }
        public async Task<IEnumerable<ProductPhoto>> UploadMany(IEnumerable<IFormFile> Image, Guid productId)
        {
            List<ProductPhoto> files = new List<ProductPhoto>();
            var result = Image.Where(i => i.Length < _fileSizeLimit & i.Length > 0);
            foreach (IFormFile img in result.Take(5))
            {
                var check = FileCheck(new BinaryReader(img.OpenReadStream()).ReadBytes(8));
                if (check == FileType.Unknown)
                    continue;

                var photoId = Guid.NewGuid();
                var relativePath = String.Concat(productId.ToString(), "--", photoId, ".", check.ToString().ToLower());
                string filePath = Path.Combine(_filePath, relativePath);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                img.CopyTo(fileStream);
                files.Add(new ProductPhoto { PhotoId = photoId, Link = String.Concat("/img/", relativePath), ProductId = productId });

            }
            return files;
        }
        public int FileCheck(IFormFile formFile)
        {
            var check = FileCheck(new BinaryReader(formFile.OpenReadStream()).ReadBytes(8));
            return ((int)check);
        }
        private FileType FileCheck(ReadOnlyMemory<byte> bytes)
        {
            if (!MemoryMarshal.TryGetArray(bytes, out var memorySegment))
                return FileType.Unknown;
            foreach (var (fileType, signatures) in _fileSignature)
            {
                var headerBytes = memorySegment.Take(signatures.Length);
                var isSignatureFound = headerBytes.SequenceEqual(signatures);
                if (!isSignatureFound) continue;
                return fileType;
            }
            return FileType.Unknown;
        }
    }
}