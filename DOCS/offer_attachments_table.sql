-- VegaAsis: teklif dosyalari icin tablo
-- Calistirmadan once veritabani yedegi alin.

CREATE TABLE IF NOT EXISTS public.offer_attachments (
  id uuid PRIMARY KEY,
  offer_id uuid NOT NULL,
  file_name character varying(255) NOT NULL,
  file_path character varying(500) NOT NULL,
  content_type character varying(100),
  file_size integer,
  created_at timestamp without time zone NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_offer_attachments_offer_id
  ON public.offer_attachments (offer_id);

-- Opsiyonel FK (tablo mevcutsa acilabilir)
-- ALTER TABLE public.offer_attachments
--   ADD CONSTRAINT fk_offer_attachments_offer
--   FOREIGN KEY (offer_id) REFERENCES public.offers(id);
