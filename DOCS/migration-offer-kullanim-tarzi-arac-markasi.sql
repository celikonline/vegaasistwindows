-- VegaAsis: offers tablosuna Kullanım Tarzı ve Araç Markası kolonları
-- RaporGrafikForm Kullanım Tarzı / Marka grafiklerinde gerçek veri kullanımı için.
-- Çalıştırmadan önce veritabanı yedeği alın.

ALTER TABLE public.offers
  ADD COLUMN IF NOT EXISTS kullanim_tarzi character varying(200),
  ADD COLUMN IF NOT EXISTS arac_markasi character varying(200);

COMMENT ON COLUMN public.offers.kullanim_tarzi IS 'Özel/Ticari/Resmi vb. kullanım tarzı';
COMMENT ON COLUMN public.offers.arac_markasi IS 'Araç markası (grafik raporu için)';
