using PlanetaryResourceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Data
{
    class EveRepository
    {
        private EvePIDataEntities _context;

        internal EveRepository()
        {
            _context = new EvePIDataEntities();
        }

        internal List<AnalysisItem> GetProductionItems(int level)
        {
            var items = _context.Items.Where(arg => arg.ProductionLevel == level).Select(item => new AnalysisItem
            {
                ProductionLevel = item.ProductionLevel,
                Product = new Models.Product
                {
                    Name = item.Name,
                    ItemId = item.ItemId
                },
                Materials = item.Inputs.Select(input => new RawMaterial
                {
                    Name = input.Name,
                    ItemId = input.ItemId
                }).ToList()
            }).ToList();

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

        internal List<TradeCategory> GetTradeCategories()
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
    }
}
