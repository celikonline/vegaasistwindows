-- VegaAsis: duyuru okundu bilgisi (kullanıcı bazlı)
-- Çalıştırmadan önce veritabanı yedeği alın.

CREATE TABLE IF NOT EXISTS public.announcement_reads (
  id uuid PRIMARY KEY,
  announcement_id uuid NOT NULL,
  user_id uuid NOT NULL,
  read_at timestamp without time zone NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_announcement_reads_user_id
  ON public.announcement_reads (user_id);

CREATE INDEX IF NOT EXISTS ix_announcement_reads_announcement_id
  ON public.announcement_reads (announcement_id);

-- Opsiyonel FK (tablolar mevcutsa açılabilir)
-- ALTER TABLE public.announcement_reads
--   ADD CONSTRAINT fk_announcement_reads_announcement
--   FOREIGN KEY (announcement_id) REFERENCES public.announcements(id);
