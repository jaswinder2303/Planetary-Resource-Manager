using PlanetaryResourceManager.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Core.Data
{
    public class EveRepository : IDisposable
    {
        private EvePIDataEntities _context;

        public EveRepository()
        {
            _context = new EvePIDataEntities();
        }

        public List<AnalysisItem> GetProductionItems(int level)
        {
            var items = _context.Items.Where(arg => arg.ProductionLevel == level).Select(item => new AnalysisItem
            {
                ProductionLevel = item.ProductionLevel,
                ProductId = item.ItemId,
                Product = new Product
                {
                    Name = item.Name,
                    ItemId = item.ItemId
                },
                Materials = item.Inputs.Select(input => new RawMaterial
                {
                    Name = input.Name,
                    ItemId = input.ItemId,
                    InputLevel = input.ProductionLevel
                }).ToList()
            }).ToList();

            //if the item is a raw material or a single input product
            foreach (var item in items)
            {
                while (item.Materials.Count < 2)
                {
                    item.Materials.Add(new RawMaterial
                    {
                        Name = "None"
                    });
                }
            }

            return items;
        }

        public List<TradeCategory> GetTradeCategories()
        {
            var items = _context.Categories.ToList();

            var categories = items.Select(arg => new TradeCategory
            {
                Name = arg.Name,
                TradeScore = arg.TradeScore,
                LastScanDate = arg.LastScanDate,
                Groups = arg.Groups.Select(group => new TradeGroup
                {
                    Name = group.Name,
                    Items = group.Commodities.Select(item => new TradeItem
                    {
                        Name = item.Name,
                        Id = item.Id
                    }).ToList()
                }).ToList()
            }).ToList();

            return categories;
        }

        public List<LoyaltyStoreItem> GetLoyaltyStoreItems()
        {
            var items = _context.LoyaltyItems.ToList();

            var storeItems = items.Select(arg => new LoyaltyStoreItem
            {
                ItemId = arg.ItemId,
                Name = arg.ItemName,
                Points = arg.LPCost,
                StorePrice = arg.ISKCost,
                Product = new Product
                {
                    Name = arg.ItemName,
                    ItemId = arg.ItemId
                },
            }).ToList();

            return storeItems;
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
