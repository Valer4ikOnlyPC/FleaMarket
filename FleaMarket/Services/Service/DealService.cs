using Domain.Core;
using Domain.Dto;
using Domain.IServices;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.Deal;
using static Domain.Models.Product;

namespace Services.Service
{
    public class DealService : IDealService
    {
        IDealRepository _dealRepository;
        IProductRepository _productRepository;
        IUserRepository _userRepository;
        public DealService(IDealRepository dealRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _dealRepository = dealRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        public async Task<Guid> Create(Deal item)
        {
            return await _dealRepository.Create(item);
        }

        public async void Delete(Guid id)
        {
            _dealRepository.Delete(id);
        }

        public async Task<IEnumerable<Deal>> GetAll()
        {
            return await _dealRepository.GetAll();
        }

        public async Task<Deal> GetById(Guid id)
        {
            return await _dealRepository.GetById(id);
        }

        public async Task<IEnumerable<DealDto>> GetByMaster(User userMaster)
        {
            var deal = await _dealRepository.GetByMaster(userMaster);
            var result = new List<DealDto>();
            foreach (var item in deal)
            {
                var dealDto = new DealDto()
                {
                    DealId = item.DealId,
                    UserMaster = item.UserMaster,
                    UserMasterName = _userRepository.GetById(item.UserMaster).GetAwaiter().GetResult().Name,
                    UserRecipient = item.UserRecipient,
                    UserRecipientName = _userRepository.GetById(item.UserRecipient).GetAwaiter().GetResult().Name,
                    ProductMaster = item.ProductMaster,
                    ProductMasterName = _productRepository.GetById(item.ProductMaster).GetAwaiter().GetResult().Name,
                    ProductRecipient = item.ProductRecipient,
                    ProductRecipientName = _productRepository.GetById(item.ProductRecipient).GetAwaiter().GetResult().Name,
                    IsActive = item.IsActive
                };
                result.Add(dealDto);
            }
            return result;
        }

        public async Task<IEnumerable<DealDto>> GetByRecipient(User userRecipient)
        {
            var deal = await _dealRepository.GetByRecipient(userRecipient);
            var result = new List<DealDto>();
            foreach (var item in deal)
            {
                var dealDto = new DealDto()
                {
                    DealId = item.DealId,
                    UserMaster = item.UserMaster,
                    UserMasterName = _userRepository.GetById(item.UserMaster).GetAwaiter().GetResult().Name,
                    UserRecipient = item.UserRecipient,
                    UserRecipientName = _userRepository.GetById(item.UserRecipient).GetAwaiter().GetResult().Name,
                    ProductMaster = item.ProductMaster,
                    ProductRecipient = item.ProductRecipient,
                    ProductRecipientName = _productRepository.GetById(item.ProductRecipient).GetAwaiter().GetResult().Name,
                    IsActive = item.IsActive
                };
                if (item.ProductMaster != Guid.Empty)
                    dealDto.ProductMasterName = _productRepository.GetById(item.ProductMaster).GetAwaiter().GetResult().Name;
                result.Add(dealDto);
            }
            return result;
        }

        public async Task<IEnumerable<DealDto>> GetByUser(User user)
        {
            var deal = await _dealRepository.GetByUser(user);
            var result = new List<DealDto>();
            foreach (var item in deal)
            {
                var dealDto = new DealDto()
                {
                    DealId = item.DealId,
                    UserMaster = item.UserMaster,
                    UserMasterName = _userRepository.GetById(item.UserMaster).GetAwaiter().GetResult().Name,
                    UserRecipient = item.UserRecipient,
                    UserRecipientName = _userRepository.GetById(item.UserRecipient).GetAwaiter().GetResult().Name,
                    ProductMaster = item.ProductMaster,
                    ProductRecipient = item.ProductRecipient,
                    ProductRecipientName = _productRepository.GetById(item.ProductRecipient).GetAwaiter().GetResult().Name,
                    IsActive = item.IsActive
                };
                if (item.ProductMaster != Guid.Empty)
                    dealDto.ProductMasterName = _productRepository.GetById(item.ProductMaster).GetAwaiter().GetResult().Name;
                result.Add(dealDto);
            }
            return result;
        }
        public async Task<int> GetByRecipientCount(User userRecipient)
        {
            return await _dealRepository.GetByRecipientCount(userRecipient);
        }
        public async Task<bool> CheckRelevant(Deal deal)
        {
            var result = _dealRepository.GetAll().GetAwaiter().GetResult().Where(d => d.UserMaster == deal.UserMaster
                && d.UserRecipient == deal.UserRecipient && d.ProductMaster == deal.ProductMaster && d.ProductRecipient == deal.ProductRecipient);
            if(result.Count()==0)
                return true;
            return false;
        }
        public async void Accepted(Guid dealId)
        {
            var deal = await _dealRepository.GetById(dealId);
            _dealRepository.Update(dealId, (int)Deal.enumIsActive.Accepted);
            _productRepository.UpdateState(deal.ProductRecipient, (int)Product.enumIsActive.InDeal);
            _productRepository.UpdateState(deal.ProductMaster, (int)Product.enumIsActive.InDeal);
            
            var dealsRecipient = await _dealRepository.GetByProduct(deal.ProductRecipient);
            foreach (Deal dealRecipient in dealsRecipient)
            {
                if(dealRecipient.DealId != dealId)
                    _dealRepository.Update(dealRecipient.DealId, (int)Deal.enumIsActive.Terminated);
            }

            var dealsMaster = await _dealRepository.GetByProduct(deal.ProductMaster);
            foreach (Deal dealMaster in dealsMaster)
            {
                if (dealMaster.DealId != dealId)
                    _dealRepository.Update(dealMaster.DealId, (int)Deal.enumIsActive.Terminated);
            }
        }
        public async void Update(Guid dealId, Deal.enumIsActive enumIsActive)
        {
            _dealRepository.Update(dealId, (int)enumIsActive);
        }
    }
}
