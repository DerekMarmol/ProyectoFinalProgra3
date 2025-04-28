using System;
using SuperBodega.Core.Models;

namespace SuperBodega.Core.Messaging
{
    public class SaleStatusChangedMessage
    {
        public int SaleId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public SaleStatus OldStatus { get; set; }
        public SaleStatus NewStatus { get; set; }
        public DateTime StatusChangeDate { get; set; }
    }
}