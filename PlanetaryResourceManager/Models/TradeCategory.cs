using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetaryResourceManager.Models
{
    class TradeCategory : BaseModel
    {
        private bool _isProcessed;

        public string Name { get; set; }
        public List<TradeGroup> Groups { get; set; }
        public int TradeScore { get; set; }
        public DateTime? LastScanDate { get; set; }

        public bool IsProcessed
        {
            get
            {
                return _isProcessed;
            }
            set
            {
                _isProcessed = value;
                RaisePropertyChanged("IsProcessed");
            }
        }
    }
}
