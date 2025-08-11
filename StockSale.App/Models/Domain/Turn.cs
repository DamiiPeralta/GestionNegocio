    using Azure;

    namespace StockSale.App.Models.Domain
    {
        public class Turn
        {
            public Guid Id { get; set; }
            public int NTurn { get; set; }
            public DateTime Date { get; set; }
            public float Initial_Cash { get; set; }

            public float Actual_Cash { get; set; }
            public float Closing_Cash { get; set; }

            public bool Is_Closed { get; set; }

            public bool IsDeleted { get; set; }

            //Relaciones
            public ICollection<Client>? Clients { get; set; }
            public ICollection<OrderBuy>? OrderBuys { get; set; }
        }
    }
