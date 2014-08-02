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

        public EveRepository()
        {
            _context = new EvePIDataEntities();
        }

        public List<AnalysisItem> GetProductionItems(int level)
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

            return items;
        }
    }
}
