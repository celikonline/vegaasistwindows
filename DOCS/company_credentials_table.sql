-- Company Credentials Table
-- Şirket portal kullanıcı adı/şifre bilgilerini saklar
-- Her kullanıcı için her şirkete ait ayrı credential kaydı tutulabilir

CREATE TABLE IF NOT EXISTS company_credentials (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    company_id VARCHAR(50) NOT NULL,
    username VARCHAR(100),
    password_encrypted VARCHAR(256),
    user_id UUID,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT NOW(),
    updated_at TIMESTAMP DEFAULT NOW(),
    UNIQUE(company_id, user_id)
);

-- Index for faster lookups
CREATE INDEX IF NOT EXISTS idx_company_credentials_company_id ON company_credentials(company_id);
CREATE INDEX IF NOT EXISTS idx_company_credentials_user_id ON company_credentials(user_id);
CREATE INDEX IF NOT EXISTS idx_company_credentials_active ON company_credentials(is_active) WHERE is_active = true;

COMMENT ON TABLE company_credentials IS 'Şirket portal giriş bilgileri';
COMMENT ON COLUMN company_credentials.company_id IS 'Şirket kodu (ak, anadolu, allianz vb.)';
COMMENT ON COLUMN company_credentials.username IS 'Portal kullanıcı adı';
COMMENT ON COLUMN company_credentials.password_encrypted IS 'Şifrelenmiş parola';
COMMENT ON COLUMN company_credentials.user_id IS 'Bu credential''ın sahibi olan kullanıcı';
COMMENT ON COLUMN company_credentials.is_active IS 'Aktif/pasif durumu';
