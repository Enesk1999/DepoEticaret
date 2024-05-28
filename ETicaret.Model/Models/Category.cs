using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ETicaret.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Kategori Adı Boş Bırakılamaz")]
        [DisplayName("Kategori Adı")]
        [MaxLength(50)]
        public string? Name { get; set; }
        [DisplayName("Görüntülenme Sayısı")]
        [Range(1, 100, ErrorMessage = "Görüntülenme Sayısı 1-100 Arasında Olmalı!")]
        public int DisplayOrder { get; set; }

        //public int ProductId { get; set; }
        //public virtual ICollection<Product> Products  { get; set; }     //N to n relitionships
    }
}
