using Repository.Core;
using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using System.Security.Cryptography;
using Npgsql;

namespace Repository.Data
{
    public class UserPasswordRepository: IUserPasswordRepository
    {
        private string connectionString = null;
        private IDbConnection db;
        public UserPasswordRepository(string conn)
        {
            connectionString = conn;
            db = new NpgsqlConnection(connectionString);
        }
        public Guid Create(string password)
        {
            UserPassword pas = new UserPassword(password);

            return db.Query<Guid>(
                "INSERT INTO UserPasswords (Password1) " +
                "VALUES(@Password) " +
                "RETURNING UserPasswordId;", new { pas.Password }).FirstOrDefault();
        }
        public bool Verification(string password, Guid passwordID)
        {
            string hashedPassword = db.Query<string>(
                "SELECT Password1 " +
                "FROM UserPasswords " +
                "WHERE UserPasswordId = @passwordID;", new { passwordID }).FirstOrDefault();
            if (hashedPassword == null)
                throw new ArgumentNullException("password");
            return VerifyHashedPassword(hashedPassword, password);
        }

        private static bool VerifyHashedPassword(string hashedPassword, string password)
        {
            byte[] buffer4;
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] src = Convert.FromBase64String(hashedPassword);
            if ((src.Length != 0x31) || (src[0] != 0))
            {
                return false;
            }
            byte[] dst = new byte[0x10];
            Buffer.BlockCopy(src, 1, dst, 0, 0x10);
            byte[] buffer3 = new byte[0x20];
            Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, dst, 0x3e8))
            {
                buffer4 = bytes.GetBytes(0x20);
            }
            return ByteArraysEqual(buffer3, buffer4);
        }
        private static bool ByteArraysEqual(byte[] b1, byte[] b2)
        {
            if (b1 == b2) return true;
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
    }
}
