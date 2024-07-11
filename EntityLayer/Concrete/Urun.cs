﻿using EntityLayer.Abstract;

namespace EntityLayer.Concrete
{

    public class Urun : BaseEntity
    {
        public string Marka { get; set; }
        public string Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public string UrunKodu { get; set; }
        public string MalzemeKodu { get; set; }
        public int StokAdedi { get; set; }

        public byte[] Urunfoto { get; set; }

        public ICollection<HayvanTurleri> HayvanTurleri { get; set; }
        public ICollection<KategoriUrun> KategoriUrunleri { get; set; }
    }
}
