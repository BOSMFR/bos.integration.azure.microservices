using BOS.Integration.Azure.Microservices.Domain.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductRequestDTO
    {
        private const int MaxLength = 20;
        private const int BarcodeLength = 13;

        [Required]
        public string PartNumber { get; set; }

        [Required, StringLength(BarcodeLength)]
        [BarcodeValidation]
        public string Barcode { get; set; }

        public int ProductId { get; set; }

        public string TypeId { get; set; }

        [Required, MaxLength(MaxLength)]
        public string Description { get; set; }

        [MaxLength(MaxLength)]
        public string Variant1 { get; set; }

        [MaxLength(MaxLength)]
        public string Variant2 { get; set; }

        [MaxLength(MaxLength)]
        public string Variant3 { get; set; }

        [MaxLength(MaxLength)]
        public string Variant4 { get; set; }

        [Required, MaxLength(MaxLength)]
        public string Variant5 { get; set; }

        public bool Fifo { get; set; }

        public bool GiveAway { get; set; }

        public bool ExtraService { get; set; }

        [Required]
        public string ErpjobId { get; set; }
    }
}
