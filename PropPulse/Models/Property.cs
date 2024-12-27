using System.ComponentModel.DataAnnotations;

namespace PropPulse.Models
{
    public class Property

    {
        // ID'si olmalı!
        public int Id { get; set; }

        // Başlık
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title can be a maximum of 100 characters.")]
        public string Title { get; set; }

        // Fiyat
        [Required(ErrorMessage = "Price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Price cannot be less than 1.")]
        public decimal Price { get; set; }

        // Alan
        [Required(ErrorMessage = "Area is required.")]
        [Range(10, 10000, ErrorMessage = "Area must be between 10 and 10,000 square meters.")]
        public int Area { get; set; }

        // Adres
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address can be a maximum of 200 characters.")]
        public string Address { get; set; }

        // Açıklama
        [StringLength(500, ErrorMessage = "Description can be a maximum of 500 characters.")]
        public string Description { get; set; }

        // Eşyalı Durum
        [Required(ErrorMessage = "You must specify whether the property is furnished.")]
        public bool IsFurnished { get; set; }

        // Fotoğraf URL'si
        //[StringLength(500, ErrorMessage = "Each photo URL should be a valid path.")]
        //public string PhotoJPG { get; set; }

        // Kullanıcı bilgisi
        [Required]
        public int UserID { get; set; }
        public User? User { get; set; }
        public List<string> Photos { get; set; } = new List<string>();

        [Required(ErrorMessage = "RoomCount is required.")]
        [Range(1, 10, ErrorMessage = "RoomCount must be between 1 and 10")]
        public int RoomCount { get; set; }
        [Required]
        public bool IsRent {  get; set; }
    }
}




