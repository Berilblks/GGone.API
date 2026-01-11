# GGone.API

GGone.API is a backend project developed for a comprehensive health and fitness application. It offers a rich feature set that allows users to manage their diets, exercises, habits, and social interactions.

## 🚀 Features

The project includes the following essential features needed for a modern fitness application:

* **🔐 Authentication (Auth):**
* User Registration and Login (JWT-based).
* Password Reset operations (with email verification code).
* Secure account deletion processes.

* **🥗 Diet and Nutrition:**
* Creating personalized diet plans.
* Weekly diet tracking.
* Monitoring current diet status.

* **🏋️ Exercise and Training:**
* Extracting exercise data (API integration).
* User-specific training plans.

* **🤖 AI Assistant (Gemini):**
* An AI chatbot that answers user questions and provides advice.

* Smart suggestions on diet and exercise.

* **🤝 Social Interaction:**
* Adding, searching, and listing friends.
* Features for interacting with your social circle.

* **🚭 Addiction Cessation:**
* Tracking addiction cessation processes.
* Monitoring progress.

* **📈 Progress and Tracking:**
* Body Mass Index (BMI) calculation and tracking.
* Weight tracking and progress graphs.

* **🏆 Gamification:**
* Badge system.
* Level and XP system to increase user motivation.

* **✅ Task Management:**
* Creating and tracking user tasks.

## 🛠️ Technologies

This project was developed using the following modern technologies and libraries:

* **Framework:** .NET 8 (ASP.NET Core Web API)
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core
* **API Documentation:** Swagger / OpenAPI
* **Artificial Intelligence:** Google Gemini AI Integration
* **Data Integration:** RapidAPI (for exercise data)
* **Authentication:** JWT (JSON Web Tokens)
* **Other:** AutoMapper, Dependency Injection

## ⚙️ Setup

To run the project in your local environment, follow these steps:

1. **Clone the Repository:**
```bash
git clone https://github.com/username/GGone.API.git
cd GGone.API
```

2. **Configuration Settings:**
Create or update the `appsettings.json` file. Make sure you include the necessary connection strings and API keys as follows:
```json
{
"ConnectionStrings": {
"DefaultConnection": "Server=...;Database=GGoneDb;Trusted_Connection=True;..."
},
"Jwt": {
"Key": "YourSecretKey...",
"Issuer": "GGone",
"Audience": "GGoneUsers"
},
"Gemini": {
"ApiKey": "API_KEY_HERE"
},
"RapidApi": {
"Key": "RAPID_API_KEY"
}
}

```
3. **Database Creation:**
Create the database by implementing Entity Framework Migrations:
```bash
dotnet ef database update
```

4. **Running the Project:**
```bash
dotnet run
```
Once the project is running, you can access the Swagger interface at `https://localhost:7157/swagger` (the port number may vary).

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
# GGone.API

GGone.API, kapsamlı bir sağlık ve fitness uygulaması için geliştirilmiş bir Backend projesidir. Kullanıcıların diyetlerini, egzersizlerini, alışkanlıklarını ve sosyal etkileşimlerini yönetmelerine olanak tanıyan zengin bir özellik seti sunar.

## 🚀 Özellikler

Proje, modern bir fitness uygulamasının ihtiyaç duyduğu aşağıdaki temel özellikleri içerir:

*   **🔐 Kimlik Doğrulama (Auth):**
    *   Kullanıcı Kaydı ve Girişi (JWT tabanlı).
    *   Şifre Sıfırlama işlemleri (E-posta doğrulama kodu ile).
    *   Güvenli hesap silme süreçleri.

*   **🥗 Diyet ve Beslenme:**
    *   Kişiselleştirilmiş diyet planları oluşturma.
    *   Haftalık diyet takibi.
    *   Mevcut diyet durumunun kontrolü.

*   **🏋️ Egzersiz ve Antrenman:**
    *   Egzersiz verilerinin çekilmesi (API entegrasyonu).
    *   Kullanıcıya özel antrenman planları.

*   **🤖 AI Asistanı (Gemini):**
    *   Kullanıcıların sorularını yanıtlayan ve tavsiyeler veren yapay zeka sohbet botu.
    *   Diyet ve egzersiz konusunda akıllı öneriler.

*   **🤝 Sosyal Etkileşim:**
    *   Arkadaş ekleme, arama ve listeleme.
    *   Sosyal çevre ile etkileşim özellikleri.

*   **🚭 Bağımlılıkla Mücadele (Addiction Cessation):**
    *   Bağımlılık bırakma süreçlerinin takibi.
    *   İlerleme durumunun izlenmesi.

*   **📈 İlerleme ve Takip:**
    *   Vücut Kitle İndeksi (BKİ) hesaplama ve takibi.
    *   Kilo takibi ve ilerleme grafikleri.

*   **🏆 Oyunlaştırma (Gamification):**
    *   Rozet (Badge) sistemi.
    *   Seviye (Level) ve XP sistemi ile kullanıcı motivasyonunun artırılması.

*   **✅ Görev Yönetimi:**
    *   Kullanıcı görevlerinin oluşturulması ve takibi.

## 🛠️ Teknolojiler

Bu proje, aşağıdaki modern teknolojiler ve kütüphaneler kullanılarak geliştirilmiştir:

*   **Framework:** .NET 8 (ASP.NET Core Web API)
*   **Veritabanı:** Microsoft SQL Server
*   **ORM:** Entity Framework Core
*   **API Dokümantasyonu:** Swagger / OpenAPI
*   **Yapay Zeka:** Google Gemini AI Entegrasyonu
*   **Veri Entegrasyonu:** RapidAPI (Egzersiz verileri için)
*   **Kimlik Doğrulama:** JWT (JSON Web Tokens)
*   **Diğer:** AutoMapper, Dependency Injection

## ⚙️ Kurulum

Projeyi yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin:

1.  **Depoyu Klonlayın:**
    ```bash
    git clone https://github.com/kullaniciadi/GGone.API.git
    cd GGone.API
    ```

2.  **Yapılandırma Ayarları:**
    `appsettings.json` dosyasını oluşturun veya güncelleyin. Aşağıdaki gibi gerekli bağlantı dizelerini ve API anahtarlarını eklediğinizden emin olun:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=...;Database=GGoneDb;Trusted_Connection=True;..."
      },
      "Jwt": {
        "Key": "GizliAnahtariniz...",
        "Issuer": "GGone",
        "Audience": "GGoneUsers"
      },
      "Gemini": {
        "ApiKey": "API_KEY_BURAYA"
      },
      "RapidApi": {
        "Key": "RAPID_API_KEY"
      }
    }
    ```

3.  **Veritabanı Oluşturma:**
    Entity Framework Migration'larını uygulayarak veritabanını oluşturun:
    ```bash
    dotnet ef database update
    ```

4.  **Projeyi Çalıştırma:**
    ```bash
    dotnet run
    ```
    Proje çalıştığında, Swagger arayüzüne `https://localhost:7157/swagger` adresinden erişebilirsiniz (port numarası değişebilir).
