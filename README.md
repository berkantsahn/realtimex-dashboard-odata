# RealTimeX Dashboard with OData

[English](#english) | [Türkçe](#türkçe)

# English

Modern, real-time dashboard application with OData support and WhatsApp-like media features.

## Features

### Backend (.NET 8.0)
- OData support for advanced querying
- MongoDB integration
- SignalR for real-time communication
- Media processing (FFmpeg & ImageSharp)
- JWT Authentication
- Repository Pattern & Unit of Work
- Clean Architecture

### Frontend (Angular 17)
- DevExtreme DataGrid with OData
- Real-time updates with SignalR
- Material Design UI
- Media viewer (images, videos, audio, documents)
- Responsive design
- TypeScript strict mode

## Prerequisites
- Node.js (v18+)
- .NET 8.0 SDK
- MongoDB (v6.0+)
- FFmpeg

## Installation

### Backend Setup
```bash
# Clone repository
git clone https://github.com/yourusername/realtimex-dashboard-odata.git
cd realtimex-dashboard-odata

# Install dependencies
cd Backend
dotnet restore

# Update MongoDB connection
# Edit appsettings.json in RealtimeX.Dashboard.API
```

### Frontend Setup
```bash
# Navigate to frontend
cd Frontend/realtimex-dashboard

# Install packages
npm install
```

## Running the Application

### Start Backend
```bash
cd Backend/RealtimeX.Dashboard.API
dotnet run
```

### Start Frontend
```bash
cd Frontend/realtimex-dashboard
ng serve
```

Access:
- Frontend: http://localhost:4200
- Backend API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

---

# Türkçe

OData desteği ve WhatsApp benzeri medya özellikleri içeren modern, gerçek zamanlı dashboard uygulaması.

## Özellikler

### Backend (.NET 8.0)
- Gelişmiş sorgulama için OData desteği
- MongoDB entegrasyonu
- Gerçek zamanlı iletişim için SignalR
- Medya işleme (FFmpeg & ImageSharp)
- JWT Kimlik Doğrulama
- Repository Pattern & Unit of Work
- Clean Architecture

### Frontend (Angular 17)
- OData destekli DevExtreme DataGrid
- SignalR ile gerçek zamanlı güncellemeler
- Material Design arayüz
- Medya görüntüleyici (resim, video, ses, döküman)
- Responsive tasarım
- TypeScript strict mod

## Gereksinimler
- Node.js (v18+)
- .NET 8.0 SDK
- MongoDB (v6.0+)
- FFmpeg

## Kurulum

### Backend Kurulumu
```bash
# Depoyu klonla
git clone https://github.com/yourusername/realtimex-dashboard-odata.git
cd realtimex-dashboard-odata

# Bağımlılıkları yükle
cd Backend
dotnet restore

# MongoDB bağlantısını güncelle
# RealtimeX.Dashboard.API içindeki appsettings.json dosyasını düzenle
```

### Frontend Kurulumu
```bash
# Frontend klasörüne git
cd Frontend/realtimex-dashboard

# Paketleri yükle
npm install
```

## Uygulamayı Çalıştırma

### Backend'i Başlat
```bash
cd Backend/RealtimeX.Dashboard.API
dotnet run
```

### Frontend'i Başlat
```bash
cd Frontend/realtimex-dashboard
ng serve
```

Erişim:
- Frontend: http://localhost:4200
- Backend API: http://localhost:5000
- Swagger: http://localhost:5000/swagger

## API Kullanımı / API Usage

### OData Sorguları / OData Queries
```http
# Filtreleme / Filtering
GET /api/realtime-data?$filter=value gt 100

# Sıralama / Sorting
GET /api/realtime-data?$orderby=timestamp desc

# Sayfalama / Pagination
GET /api/realtime-data?$skip=10&$top=10

# İlişkili veri / Related data
GET /api/announcements?$expand=attachments
```

### Medya İşlemleri / Media Operations
```typescript
// Medya yükleme / Upload media
const formData = new FormData();
formData.append('file', file);
await chatService.uploadMedia(formData);

// Medya görüntüleme / View media
this.dialog.open(MediaViewerComponent, {
  data: mediaMessage
});
```

### Gerçek Zamanlı İletişim / Real-time Communication
```typescript
// Mesaj alma / Receive messages
this.realTimeService.messageReceived.subscribe(message => {
  // Mesajı işle / Handle message
});

// Mesaj gönderme / Send message
await this.chatService.sendMessage({
  content: 'Merhaba / Hello',
  receiverId: userId
});
```

## Katkıda Bulunma / Contributing

1. Fork'layın / Fork it
2. Feature branch oluşturun / Create feature branch (`git checkout -b feature/amazing-feature`)
3. Değişiklikleri commit edin / Commit changes (`git commit -m 'Add some amazing feature'`)
4. Branch'i push edin / Push to branch (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun / Create Pull Request

## Lisans / License

Bu proje MIT lisansı altında lisanslanmıştır - Detaylar için LICENSE dosyasına bakın.
This project is licensed under the MIT License - see the LICENSE file for details.