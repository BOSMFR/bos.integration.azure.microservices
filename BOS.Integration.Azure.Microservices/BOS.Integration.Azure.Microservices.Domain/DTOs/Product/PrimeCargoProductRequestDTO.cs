using BOS.Integration.Azure.Microservices.Domain.Enums;
using BOS.Integration.Azure.Microservices.Domain.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace BOS.Integration.Azure.Microservices.Domain.DTOs.Product
{
    public class PrimeCargoProductRequestDTO
    {
        private const int PartNumberMaxLength = 20;
        private const int VariantMaxLength = 25;
        private const int DescriptionMaxLength = 50;
        private const int BarcodeLength = 13;

        [Required, MaxLength(PartNumberMaxLength)]
        public string PartNumber { get; set; }

        [Required, StringLength(BarcodeLength)]
        [BarcodeValidation]
        public string Barcode { get; set; }

        public int? ProductId { get; set; }

        public PrimeCargoProductType? TypeId { get; set; }

        [Required, MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        [MaxLength(VariantMaxLength)]
        public string Variant1 { get; set; }

        [MaxLength(VariantMaxLength)]
        public string Variant2 { get; set; }

        [MaxLength(VariantMaxLength)]
        public string Variant3 { get; set; }

        [MaxLength(VariantMaxLength)]
        public string Variant4 { get; set; }

        [Required, MaxLength(VariantMaxLength)]
        public string Variant5 { get; set; }

        public bool Fifo { get; set; }

        public bool GiveAway { get; set; }

        public bool ExtraService { get; set; }

        [Required]
        public string ErpjobId { get; set; }
    }
}
