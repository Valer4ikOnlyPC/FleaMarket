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
        private readonly IDealRepository _dealRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFavoritesService _favoritesService;
        public DealService(IDealRepository dealRepository, IProductRepository productRepository, IUserRepository userRepository, IFavoritesService favoritesService)
        {
            _dealRepository = dealRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _favoritesService = favoritesService;
        }
        public async Task<Guid> Create(Deal item)
        {
            return await _dealRepository.Create(item);
        }

        public async Task Delete(Guid id)
        {
            await _dealRepository.Delete(id);
        }

        public async Task<IEnumerable<Deal>> GetAll()
        {
            return await _dealRepository.GetAll();
        }

        public async Task<Deal> GetById(Guid id)
        {
            var deal = await _dealRepository.GetById(id);
            return deal;
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
                    UserMasterName = (await _userRepository.GetById(item.UserMaster)).Name,
                    UserRecipient = item.UserRecipient,
                    UserRecipientName = (await _userRepository.GetById(item.UserRecipient)).Name,
                    ProductMaster = item.ProductMaster,
                    ProductMasterName = (await _productRepository.GetById(item.ProductMaster)).Name,
                    ProductRecipient = item.ProductRecipient,
                    ProductRecipientName = (await _productRepository.GetById(item.ProductRecipient)).Name,
                    IsActive = item.IsActive,
                    Date = item.Date
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
                    UserMasterName = (await _userRepository.GetById(item.UserMaster)).Name,
                    UserRecipient = item.UserRecipient,
                    UserRecipientName = (await _userRepository.GetById(item.UserRecipient)).Name,
                    ProductMaster = item.ProductMaster,
                    ProductRecipient = item.ProductRecipient,
                    ProductRecipientName = (await _productRepository.GetById(item.ProductRecipient)).Name,
                    IsActive = item.IsActive,
                    Date = item.Date
                };
                if (item.ProductMaster != Guid.Empty)
                    dealDto.ProductMasterName = (await _productRepository.GetById(item.ProductMaster)).Name;
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
                    UserMasterName = (await _userRepository.GetById(item.UserMaster)).Name,
                    UserRecipient = item.UserRecipient,
                    UserRecipientName = (await _userRepository.GetById(item.UserRecipient)).Name,
                    ProductMaster = item.ProductMaster,
                    ProductRecipient = item.ProductRecipient,
                    ProductRecipientName = (await _productRepository.GetById(item.ProductRecipient)).Name,
                    IsActive = item.IsActive,
                    Date = item.Date
                };
                if (item.ProductMaster != Guid.Empty)
                    dealDto.ProductMasterName = (await _productRepository.GetById(item.ProductMaster)).Name;
                result.Add(dealDto);
            }
            return result;
        }

        public async Task<IEnumerable<DealProductDto>> GetDealProductDtoByUser(User user)
        {
            var deal = await _dealRepository.GetByUser(user);
            var result = new List<DealProductDto>();
            foreach (var item in deal)
            {
                var dealDto = new DealProductDto()
                {
                    DealId = item.DealId,
                    UserMaster = item.UserMaster,
                    UserMasterName = (await _userRepository.GetById(item.UserMaster)).Name,
                    UserRecipient = item.UserRecipient,
                    UserRecipientName = (await _userRepository.GetById(item.UserRecipient)).Name,
                    ProductRecipient = await _productRepository.GetById(item.ProductRecipient),
                    IsActive = item.IsActive,
                    Date = item.Date
                };
                if (item.ProductMaster != Guid.Empty)
                    dealDto.ProductMaster = await _productRepository.GetById(item.ProductMaster);
                result.Add(dealDto);
            }
            return result;
        }
        public async Task<int> GetByRecipientCount(User userRecipient)
        {
            return await _dealRepository.GetByRecipientCount(userRecipient);
        }
        public async Task<IEnumerable<Deal>> GetByProductId(Guid productId)
        {
            return await _dealRepository.GetByProduct(productId);
        }
        public async Task<bool> CheckRelevant(Deal deal)
        {
            var result = (await _dealRepository.GetAll()).Where(d => d.UserMaster == deal.UserMaster
                && d.UserRecipient == deal.UserRecipient && d.ProductMaster == deal.ProductMaster && d.ProductRecipient == deal.ProductRecipient);
            return !result.Any();
        }
        public async Task Accepted(Guid dealId)
        {
            var deal = await _dealRepository.GetById(dealId);
            if (deal == null)
                throw new ErrorModel(400, "Deal not found");
            await _dealRepository.Update(dealId, (int)DealState.Accepted);
            await _productRepository.UpdateState(deal.ProductRecipient, (int)ProductState.InDeal);
            await _productRepository.UpdateState(deal.ProductMaster, (int)ProductState.InDeal);
            await _dealRepository.UpdateDate(dealId);

            var favoriteProduct = (await _favoritesService.GetAll()).Where(x => x.ProductId == deal.ProductMaster | x.ProductId == deal.ProductRecipient);
            foreach (var favorite in favoriteProduct)
            {
                await _favoritesService.Delete(favorite.FavoriteId);
            }

            var dealsRecipient = new List<Deal>();
            if (deal.ProductRecipient != Guid.Empty) 
                dealsRecipient = (await _dealRepository.GetByProduct(deal.ProductRecipient)).ToList();
            foreach (Deal dealRecipient in dealsRecipient)
            {
                if(dealRecipient.DealId != dealId)
                    await _dealRepository.Update(dealRecipient.DealId, (int)DealState.Terminated);
            }

            var dealsMaster = new List<Deal>();
            if (deal.ProductMaster != Guid.Empty) 
                dealsMaster = (await _dealRepository.GetByProduct(deal.ProductMaster)).ToList();
            foreach (Deal dealMaster in dealsMaster)
            {
                if (dealMaster.DealId != dealId)
                    await _dealRepository.Update(dealMaster.DealId, (int)DealState.Terminated);
            }
        }
        public async Task Update(Guid dealId, DealState enumIsActive)
        {
            await _dealRepository.Update(dealId, (int)enumIsActive);
        }
    }
}
