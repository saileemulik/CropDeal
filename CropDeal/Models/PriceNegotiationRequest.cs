using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropDeal.Models
{
    public class PriceNegotiationRequest
    {
        public Guid Id { get; set; }
        [ForeignKey("CropListing")]
        public Guid ListingId { get; set; }
        [ForeignKey("Dealer")]
        public Guid DealerId { get; set; }
        [ForeignKey("Farmer")]
        public Guid FarmerId { get; set; }
        public int Quantity { get; set; }
        public float NegotiatedPricePerKg { get; set; }
        public float TotalPrice { get; set; }
        public PriceStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [JsonIgnore]
        public User Dealer { get; set; }
        [JsonIgnore]
        public User Farmer { get; set; }

        [JsonIgnore]
        public CropListing CropListing { get; set; }
    }
}
public enum PriceStatus
{
    Accepted,
    Pending,
    Rejected
}