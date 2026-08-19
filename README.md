# 📅 Çalışma Takip

**Çalışma Takip**, günlük ve haftalık çalışma planlarını oluşturmak, tamamlanan görevleri takip etmek, geçmiş çalışma kayıtlarını incelemek ve çalışma performansını istatistiklerle değerlendirmek için geliştirilmiş bir **Windows masaüstü uygulamasıdır**.

Uygulama **.NET 8, WPF, MVVM, Entity Framework Core ve SQLite** kullanılarak geliştirilmiştir.

---

## 🎯 Projenin Amacı

Çalışma Takip'in temel amacı günlük çalışma düzenini tek bir masaüstü uygulaması üzerinden planlamak ve takip etmektir.

Kullanıcı;

* Hafta içi çalışma planı oluşturabilir
* Hafta sonu için farklı çalışma planı oluşturabilir
* Günlük çalışma durumunu takip edebilir
* Tamamlanan görevleri işaretleyebilir
* Önceki çalışma kayıtlarını inceleyebilir
* Takvim üzerinden günlerin durumunu görüntüleyebilir
* Çalışma istatistiklerini takip edebilir

---

# ✨ Temel Özellikler

## 📋 Haftalık Planlama

Uygulama çalışma programının önceden tanımlanabilmesini sağlar.

Planlar gün ve saat bazlı çalışma bloklarından oluşur.

```text
Weekly Plan
│
├── Weekday Plan
│   ├── Start Time
│   ├── End Time
│   └── Task
│
└── Weekend Plan
    ├── Start Time
    ├── End Time
    └── Task
```

Hafta içi ve hafta sonu planlarının birbirinden ayrılması sayesinde farklı çalışma rutinleri tanımlanabilir.

---

## ⏰ Zaman Aralığı Bazlı Çalışma Planı

Plan öğeleri başlangıç ve bitiş saatleriyle oluşturulur.

Örnek:

```text
09:00 - 10:00    İngilizce
10:15 - 12:00    Yazılım Geliştirme
13:00 - 14:00    Teknik Çalışma
14:30 - 16:00    Proje Geliştirme
```

Bu yapı çalışma gününün belirli zaman bloklarına ayrılmasını sağlar.

---

# ✅ Günlük Çalışma Takibi

Uygulamanın temel bölümlerinden biri günlük çalışma takip ekranıdır.

Seçilen tarih için uygun plan otomatik olarak belirlenir.

```text
Selected Date
      ↓
Weekday / Weekend Detection
      ↓
Plan Template
      ↓
Daily Tracking
```

Günlük plan üzerindeki her çalışma maddesi ayrı ayrı tamamlandı olarak işaretlenebilir.

```text
☑ Completed

☐ Not Completed
```

Günlük takip kayıtları veritabanına kaydedilir ve daha sonra tekrar açılabilir.

---

## 📝 Günlük Notlar

Her çalışma günü için ek not tutulabilir.

Bu alan;

* Gün değerlendirmesi
* Eksik kalan çalışmalar
* Bir sonraki güne aktarılacak işler
* Çalışma sırasında alınan notlar

gibi bilgilerin saklanması için kullanılabilir.

---

# 📆 Takvim Görünümü

Uygulama çalışma kayıtlarını takvim üzerinden inceleyebilmek için ayrı bir takvim yapısı içerir.

Takvim ekranı sayesinde kullanıcı çalışma geçmişini gün bazlı olarak değerlendirebilir.

```text
Calendar
│
├── Day
├── Tracking State
└── Statistics
```

Takvim altyapısı günlük takip sistemiyle birlikte çalışır.

---

# 📊 İstatistikler

Çalışma Takip yalnızca plan oluşturmayı değil, yapılan çalışmaların sonuçlarını değerlendirmeyi de hedefler.

Uygulamada ayrı bir istatistik katmanı bulunmaktadır.

Bu yapı günlük takip kayıtlarından elde edilen verilerin özetlenmesine ve çalışma performansının daha kolay incelenmesine olanak sağlar.

---

# 🕒 Takip Geçmişi

Geçmiş çalışma kayıtları ayrı bir servis ve görünüm üzerinden incelenebilir.

Bu sayede kullanıcı geçmiş tarihlerde:

* Hangi planın uygulandığını
* Hangi çalışmaların tamamlandığını
* Günlük takip durumunu
* Çalışma geçmişini

inceleyebilir.

---

# 🧠 Günlük Plan Mantığı

Günlük çalışma ekranı seçilen tarihe göre uygun plan şablonunu belirler.

```text
Date Selected
     ↓
PlanTemplateKindResolver
     ↓
Weekday / Weekend
     ↓
Load Plan Template
     ↓
Daily Tracking Rows
```

Eğer seçilen tarih için daha önce kayıt oluşturulmuşsa mevcut günlük takip kaydı yüklenir.

```text
Selected Date
     ↓
Saved Track Exists?
   /             \
 Yes              No
  ↓                ↓
Load Saved      Load Template
Tracking        Plan
```

Bu sayede günlük çalışma sırasında yapılan değişiklikler ve tamamlanma durumları korunur.

---

# 💾 Veri Saklama

Uygulama verilerini yerel olarak **SQLite** veritabanında saklar.

Veri erişim katmanında:

```text
WPF
 ↓
MVVM
 ↓
Service Layer
 ↓
Entity Framework Core
 ↓
SQLite
```

mimarisi kullanılmaktadır.

Günlük takip kayıtları tarih bazlı saklanır.

Her günlük kaydın altında ilgili çalışma maddeleri bulunur.

```text
DailyPlanTrackHeader
│
├── TrackDate
├── TemplateKind
├── Note
│
└── DailyPlanTrackItems
    ├── StartTime
    ├── EndTime
    ├── Title
    ├── SortOrder
    └── IsCompleted
```

---

# 🏗️ Mimari

Proje katmanlı ve MVVM odaklı bir yapı kullanmaktadır.

```text
CalismaTakip.App
│
├── Data
│
├── Helpers
│
├── Models
│   └── Dtos
│
├── Services
│
├── Themes
│
├── ViewModels
│
├── Views
│
├── App.xaml
└── App.xaml.cs
```

---

## Models

Uygulamanın veri modellerini içerir.

Başlıca modeller:

```text
DailyPlanTrackHeader
DailyPlanTrackItem
PlanKind
PlanTemplateItem
PlanTemplateKind
TimeSlot
WeeklyPlanItem
```

---

## Services

Business logic ve veri erişim işlemleri servis katmanında ayrıştırılmıştır.

```text
CalendarStatisticsService
DailyTrackingService
TakipGecmisiService
WeeklyPlanService
```

Servisler interface'ler üzerinden kullanılmaktadır:

```text
ICalendarStatisticsService
IDailyTrackingService
ITakipGecmisiService
IWeeklyPlanService
```

Bu yapı UI katmanının doğrudan veritabanı işlemleriyle uğraşmasını engeller.

---

## ViewModels

UI state ve kullanıcı işlemleri MVVM yaklaşımıyla ViewModel katmanında yönetilir.

Başlıca ViewModel'ler:

```text
MainViewModel

DailyTrackingViewModel
DailyPlanTrackRowViewModel

WeekdayPlanViewModel
WeekdayPlanRowViewModel

WeekendPlanViewModel
WeekendPlanRowViewModel

CalendarViewModel
CalendarDayCellViewModel

StatisticsViewModel

TakipGecmisiViewModel
TakipGecmisiRowViewModel
```

---

# 🧩 MVVM

Projede **CommunityToolkit.Mvvm** kullanılmaktadır.

Temel akış:

```text
View
 ↓
ViewModel
 ↓
Service
 ↓
Entity Framework Core
 ↓
SQLite
```

Bu yaklaşım sayesinde kullanıcı arayüzü, uygulama mantığı ve veri erişimi birbirinden ayrılmıştır.

---

# 🛠️ Teknoloji Stack'i

| Teknoloji               | Kullanım                 |
| ----------------------- | ------------------------ |
| C#                      | Ana programlama dili     |
| .NET 8                  | Uygulama platformu       |
| WPF                     | Windows masaüstü arayüzü |
| XAML                    | UI tanımları             |
| MVVM                    | Uygulama mimarisi        |
| CommunityToolkit.Mvvm   | MVVM altyapısı           |
| Entity Framework Core 8 | Veri erişimi             |
| SQLite                  | Yerel veritabanı         |
| Dependency Injection    | Servis yönetimi          |

---

# ⚙️ Gereksinimler

Projeyi geliştirme ortamında çalıştırmak için:

```text
Windows
.NET 8 SDK
Visual Studio 2022
```

önerilir.

---

# 🚀 Kurulum

Repository'yi klonlayın:

```bash
git clone https://github.com/HsnEmre/CalismaTakip.git
```

Proje dizinine geçin:

```bash
cd CalismaTakip
```

Solution dosyasını Visual Studio ile açın:

```text
CalismaTakip.sln
```

NuGet paketlerini restore ettikten sonra projeyi çalıştırabilirsiniz.

---

# 📦 Kullanılan NuGet Paketleri

Projede kullanılan temel paketler:

```text
CommunityToolkit.Mvvm 8.3.2
Microsoft.EntityFrameworkCore.Sqlite 8.0.11
Microsoft.EntityFrameworkCore.Design 8.0.11
Microsoft.Extensions.DependencyInjection 8.0.1
```

---

# 🔒 Veri Gizliliği

Çalışma verileri yerel SQLite veritabanında tutulduğu için uygulamanın temel çalışma yapısı harici bir cloud servisine bağımlı değildir.

Bu yaklaşım kişisel çalışma planlarının ve takip kayıtlarının yerel ortamda tutulmasına olanak sağlar.

---

# 🚧 Geliştirme Durumu

Projenin mevcut yapısında temel çalışma takip altyapısı bulunmaktadır:

```text
Weekly Planning
      ↓
Daily Tracking
      ↓
Calendar
      ↓
Tracking History
      ↓
Statistics
```

Proje geliştirilmeye ve yeni özelliklerle genişletilmeye uygundur.

---

# 💡 Geliştirilebilecek Özellikler

İlerleyen sürümlerde eklenebilecek bazı özellikler:

* Bildirim ve hatırlatıcı sistemi
* Pomodoro / çalışma sayacı
* Hedef bazlı çalışma takibi
* Haftalık ve aylık gelişmiş raporlar
* Grafik tabanlı performans analizi
* Veri dışa aktarma
* Yedekleme / geri yükleme
* Tema seçeneklerinin genişletilmesi

---

# 📄 License

Henüz açık kaynak lisansı belirlenmemiştir.

Bir lisans eklenene kadar repository içeriğinin kullanım ve dağıtım hakları otomatik olarak açık kaynak lisansı kapsamında değerlendirilmemelidir.
