# Sanatik - Dijital Sanat Galerisi

Sanatik, sanatsal fotoğrafların paylaşıldığı, keşfedildiği ve etkileşime girildiği modern bir web platformudur. Kullanıcıların (sanatseverlerin) eserlerini sergileyebileceği, diğer kullanıcıların eserlerini inceleyip yorumlayabileceği ve beğenebileceği bir topluluk oluşturmayı hedefler.

## 🚀 Proje Hakkında

Bu proje, ASP.NET Core MVC mimarisi kullanılarak geliştirilmiş, yaşayan ve dinamik bir web uygulamasıdır. SQL Server veritabanı altyapısı ve Entity Framework Core ile güçlü bir backend yapısına sahiptir.

### Öne Çıkan Özellikler

- **Kullanıcı Yönetimi & Kimlik Doğrulama:**
  - Güvenli üyelik sistemi (Kayıt Ol / Giriş Yap).
  - Rol tabanlı yetkilendirme (Admin ve Standart Kullanıcı).
  
- **Fotoğraf Paylaşımı:**
  - Yüksek kaliteli fotoğraf yükleme.
  - Fotoğraflara başlık, açıklama ve kategori ekleme.
  
- **Etkileşim:**
  - **Beğeni Sistemi:** Beğendiğiniz eserleri favorilerinize ekleyin.
  - **Yorum Sistemi:** Eserler hakkında düşüncelerinizi paylaşın ve sanatçılarla iletişim kurun.
  
- **Keşfet (Feed):**
  - Masonry Grid (duvar) yapısı ile modern ve akıcı fotoğraf akışı.
  - Rastgele keşfet algoritması.

- **Admin Paneli:**
  - Kullanıcıları yönetim ve denetleme.
  - İçerik moderasyonu.

## 🛠️ Teknolojiler

- **Backend:** .NET 10.0, ASP.NET Core MVC
- **Veritabanı:** Microsoft SQL Server, Entity Framework Core 10.0
- **Frontend:** HTML5, CSS3 (Premium Dark Theme), Javascript, Bootstrap
- **Kimlik Doğrulama:** ASP.NET Core Identity

## ⚙️ Kurulum ve Çalıştırma

Projeyi yerel makinenizde çalıştırmak için aşağıdaki adımları takip edebilirsiniz.

### Gereksinimler

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (LocalDB veya Full Instance)

### Adımlar

1. **Projeyi Klonlayın:**
   ```bash
   git clone https://github.com/kullaniciadi/Sanatik.git
   cd Sanatik
   ```

2. **Veritabanı Bağlantısını Yapılandırın:**
   `appsettings.json` dosyasındaki `ConnectionStrings` bölümünü kendi veritabanı ayarlarınıza göre kontrol edin. Varsayılan olarak yerel SQL Server bağlantısı kullanır.

3. **Veritabanını Oluşturun (Migration):**
   Terminali proje dizininde açın ve aşağıdaki komutu çalıştırarak veritabanını güncelleyin:
   ```bash
   dotnet ef database update
   ```

4. **Uygulamayı Çalıştırın:**
   ```bash
   dotnet run
   ```
   Tarayıcınızda verilen adrese (örn: `https://localhost:7154`) gidin.

## 🔑 Varsayılan Hesap Bilgileri

Proje ilk kez çalıştırıldığında, veritabanına otomatik olarak örnek veriler ve bir yönetici hesabı eklenir (`DbSeeder` sınıfı aracılığıyla).

- **Admin Kullanıcısı:** `admin@sanatik.com`
- **Şifre:** `Admin123!`

## 🤝 Katkıda Bulunma

1. Bu projeyi forklayın.
2. Yeni bir özellik dalı (feature branch) oluşturun (`git checkout -b ozellik/YeniOzellik`).
3. Değişikliklerinizi commit edin (`git commit -m 'Yeni özellik eklendi'`).
4. Dalınızı pushlayın (`git push origin ozellik/YeniOzellik`).
5. Bir Pull Request oluşturun.

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) ile lisanslanmıştır.
