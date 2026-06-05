# Dokumentasi Belajar CI/CD
> **Author:** Martin Minggu  
> **Repo:** [BelajarCi-CD](https://github.com/MartinMinggu/BelajarCi-CD)  
> **Stack:** .NET 8, GitHub Actions, Docker, GHCR, Railway  
> **Durasi:** Fase 1–4 (Minggu 1–6)

---

## Daftar Isi
1. [Konsep Dasar CI/CD](#1-konsep-dasar-cicd)
2. [Tools yang Digunakan](#2-tools-yang-digunakan)
3. [Struktur Project](#3-struktur-project)
4. [Fase 2 — GitHub Actions Pipeline](#4-fase-2--github-actions-pipeline)
5. [Fase 3 — Docker dalam Pipeline](#5-fase-3--docker-dalam-pipeline)
6. [Fase 4 — Deploy Otomatis ke Railway](#6-fase-4--deploy-otomatis-ke-railway)
7. [Alur Lengkap Pipeline](#7-alur-lengkap-pipeline)
8. [Kosakata Penting](#8-kosakata-penting)
9. [Pelajaran & Gotchas](#9-pelajaran--gotchas)
10. [Fase 5 — Next Steps](#10-fase-5--next-steps)

---

## 1. Konsep Dasar CI/CD

### Kenapa CI/CD ada?
Sebelum CI/CD, developer kerja di branch masing-masing lalu merge sekaligus di akhir minggu — hasilnya merge conflict, bug yang baru ketahuan terlambat, dan "integration hell". CI/CD hadir untuk memutus siklus itu.

### Tiga Prinsip Utama
| Prinsip | Artinya |
|---|---|
| **Fail fast** | Lebih baik tahu ada bug 5 menit setelah push daripada 5 hari kemudian |
| **Pipeline sebagai kode** | Instruksi build/test/deploy disimpan di file YAML di dalam repo — bisa di-review dan di-version |
| **Green build = selalu siap** | Kalau pipeline hijau, kode di `main` bisa di-deploy kapan saja |

### Perbedaan CI, Continuous Delivery, dan Continuous Deployment

```
CI (Continuous Integration)
  → Setiap push: otomatis build + test
  → Tujuan: cepat detect bug

Continuous Delivery
  → Setelah CI lulus: kode otomatis siap di staging
  → Deploy ke production: masih butuh tombol manual

Continuous Deployment
  → Setelah CI lulus: otomatis deploy ke production
  → Tidak ada sentuhan manusia sama sekali
```

### Kenapa pipeline berhenti saat build gagal?
Prinsip **fail fast** — kalau build saja sudah gagal, tidak ada gunanya menjalankan ratusan unit test. Pipeline berhenti lebih awal supaya feedback lebih cepat dan resource tidak terbuang.

---

## 2. Tools yang Digunakan

| Tool | Fungsi | Gratis? |
|---|---|---|
| **GitHub Actions** | CI/CD pipeline — build, test, docker | 2000 menit/bulan |
| **GHCR** | GitHub Container Registry — simpan Docker image | Gratis untuk public repo |
| **Docker** | Containerize aplikasi | Gratis |
| **Railway** | Platform cloud untuk deploy & hosting | 30 hari / $5 credit |

### Kenapa GitHub Actions untuk pemula?
- Tidak perlu install apapun
- Tidak perlu daftar akun baru
- Cukup buat file `.github/workflows/ci.yml` di repo
- Untuk .NET sudah ada `actions/setup-dotnet` yang tinggal pakai

---

## 3. Struktur Project

```
BelajarCi-CD/                          ← root repository
├── .github/
│   └── workflows/
│       └── ci.yml                     ← pipeline CI/CD (dibaca GitHub otomatis)
└── MathService/                       ← project .NET
    ├── MathService.sln
    ├── Dockerfile
    ├── railway.toml
    ├── src/
    │   └── MathService/
    │       ├── MathService.csproj
    │       └── Program.cs
    └── tests/
        └── MathService.Tests/
            ├── MathService.Tests.csproj
            └── MathTests.cs
```

> ⚠️ **Penting:** File `.github/workflows/ci.yml` harus berada di **root repo**, bukan di dalam subfolder project. GitHub Actions hanya membaca path ini.

---

## 4. Fase 2 — GitHub Actions Pipeline

### Cara membuat project .NET

```bash
mkdir MathService && cd MathService
dotnet new sln -n MathService
dotnet new webapi -n MathService -o src/MathService --no-openapi
dotnet new xunit -n MathService.Tests -o tests/MathService.Tests
dotnet sln add src/MathService/MathService.csproj
dotnet sln add tests/MathService.Tests/MathService.Tests.csproj
dotnet add tests/MathService.Tests/MathService.Tests.csproj reference src/MathService/MathService.csproj
```

### Program.cs — Minimal API

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "MathService is running!");
app.MapGet("/add", (int a, int b) => new { result = MathLogic.Add(a, b) });
app.MapGet("/multiply", (int a, int b) => new { result = MathLogic.Multiply(a, b) });

app.Run();

public static class MathLogic
{
    public static int Add(int a, int b) => a + b;
    public static int Multiply(int a, int b) => a * b;
}

public partial class Program { }
```

### MathTests.cs — Unit Tests

```csharp
public class MathTests
{
    [Fact]
    public void Add_TwoNumbers_ReturnsCorrectSum()
    {
        var result = MathLogic.Add(3, 7);
        Assert.Equal(10, result);
    }

    [Fact]
    public void Multiply_TwoNumbers_ReturnsCorrectProduct()
    {
        var result = MathLogic.Multiply(4, 5);
        Assert.Equal(20, result);
    }

    [Fact]
    public void Add_NegativeNumbers_Works()
    {
        var result = MathLogic.Add(-3, -7);
        Assert.Equal(-10, result);
    }
}
```

### ci.yml — Pipeline awal (CI only)

```yaml
name: CI Pipeline

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    defaults:
      run:
        working-directory: MathService   # penting! karena project ada di subfolder

    steps:
      - name: Checkout kode
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release --verbosity normal
```

### Anatomi YAML

| Bagian | Artinya |
|---|---|
| `name:` | Nama pipeline yang muncul di tab Actions |
| `on: push / pull_request` | Kapan pipeline dipicu |
| `jobs:` | Kumpulan pekerjaan — setiap job jalan di mesin terpisah |
| `runs-on: ubuntu-latest` | Mesin virtual gratis yang disediakan GitHub |
| `defaults.run.working-directory` | Folder kerja untuk semua step `run:` |
| `steps:` | Urutan langkah dalam satu job |
| `uses:` | Pakai action yang sudah dibuat orang lain |
| `run:` | Jalankan perintah shell biasa |

### Gotcha yang ditemukan
- **Error `MSB1003`** — terjadi karena `dotnet restore` dijalankan di root repo, tapi `.sln` ada di subfolder. Fix: tambahkan `defaults.run.working-directory: MathService` di job.
- **`.github/workflows/` harus di root repo** — bukan di dalam subfolder project.

---

## 5. Fase 3 — Docker dalam Pipeline

### Dockerfile (Multi-stage build)

```dockerfile
# Stage 1: Build — pakai SDK yang lengkap
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY MathService.sln .
COPY src/MathService/MathService.csproj src/MathService/
COPY tests/MathService.Tests/MathService.Tests.csproj tests/MathService.Tests/
RUN dotnet restore

COPY . .
RUN dotnet publish src/MathService/MathService.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Runtime — hanya runtime, image jauh lebih kecil
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "MathService.dll"]
```

### Kenapa multi-stage build?
SDK .NET berukuran ~700MB, sedangkan runtime hanya ~200MB. Dengan multi-stage, image production hanya berisi runtime — lebih kecil, lebih aman, lebih cepat di-pull.

### Test Docker secara lokal

```bash
cd MathService
docker build -t mathservice:local .
docker run -p 8080:8080 mathservice:local
# Buka http://localhost:8080
```

### Pull image dari GHCR

```bash
docker run -d -p 8080:8080 --name math-api ghcr.io/USERNAME/mathservice:latest
```

### ci.yml — Tambah job Docker

```yaml
  docker:
    runs-on: ubuntu-latest
    needs: build-and-test                          # hanya jalan kalau CI lulus
    if: github.ref == 'refs/heads/main' && github.event_name == 'push'

    steps:
      - name: Checkout kode
        uses: actions/checkout@v4

      - name: Login ke GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}    # otomatis tersedia, tidak perlu setup

      - name: Build dan push Docker image
        uses: docker/build-push-action@v5
        with:
          context: ./MathService
          push: true
          tags: ghcr.io/${{ github.repository_owner }}/mathservice:latest
```

### Poin penting job Docker
- `needs: build-and-test` — Docker image hanya dibuild kalau test lulus
- `if: github.ref == 'refs/heads/main'` — hanya jalan saat push ke main, bukan PR
- `GITHUB_TOKEN` — secret otomatis tersedia di setiap repo GitHub, tidak perlu dibuat manual

---

## 6. Fase 4 — Deploy Otomatis ke Railway

### Setup Railway
1. Daftar di [railway.app](https://railway.app) dengan akun GitHub
2. Klik **"New Project"** → **"Deploy from GitHub repo"**
3. Pilih repo `BelajarCi-CD`
4. Masuk ke **Settings** service → set **Root Directory** ke `MathService`
5. Klik **Settings** → **Networking** → **"Generate Domain"** untuk mendapat URL publik

### railway.toml

```toml
[build]
builder = "DOCKERFILE"
dockerfilePath = "Dockerfile"

[deploy]
startCommand = "dotnet MathService.dll"
healthcheckPath = "/"
healthcheckTimeout = 30
restartPolicyType = "ON_FAILURE"
```

### Pattern yang dipakai (GitHub Actions + Railway native)

Railway di-connect langsung ke GitHub repo — setiap push ke `main`, Railway otomatis rebuild dan redeploy. GitHub Actions tetap handle CI (test) dan packaging (Docker ke GHCR). Dua sistem bekerja paralel.

### Gotcha yang ditemukan
- **"Waiting for runner" lebih dari 15 menit** — terjadi saat mencoba deploy via Railway CLI dari pipeline. Solusi: pakai Railway native GitHub integration, jauh lebih stabil.
- **"Build failed" di Railway** — karena Root Directory belum di-set ke `MathService`. Railway mencoba build dari root repo padahal Dockerfile ada di subfolder.

---

## 7. Alur Lengkap Pipeline

```
Developer: git push ke main
           ↓
GitHub Actions Job 1: build-and-test
  - dotnet restore
  - dotnet build --configuration Release
  - dotnet test (3 tests)
           ↓ (kalau semua lulus)
GitHub Actions Job 2: docker
  - login ke ghcr.io
  - docker build (multi-stage)
  - docker push → ghcr.io/username/mathservice:latest
           ↓ (paralel)
Railway: detect push ke main
  - pull latest code
  - rebuild container
  - redeploy otomatis
           ↓
https://xxx.railway.app — langsung update!
```

### Kondisi proteksi
- Job `docker` tidak jalan kalau `build-and-test` gagal
- Railway tidak deploy kalau build gagal
- Pull Request hanya men-trigger `build-and-test`, tidak push image ke registry

---

## 8. Kosakata Penting

| Istilah | Artinya |
|---|---|
| **Pipeline** | Rangkaian langkah otomatis yang jalan setelah push |
| **Job** | Satu kelompok langkah dalam pipeline, jalan di mesin terpisah |
| **Step** | Satu perintah tunggal dalam sebuah job |
| **Runner** | Mesin virtual yang menjalankan pipeline (disediakan GitHub gratis) |
| **Artifact** | File hasil build yang diteruskan antar job |
| **Trigger** | Kondisi yang memicu pipeline jalan (push, PR, jadwal) |
| **Green build** | Pipeline selesai tanpa ada yang gagal |
| **Broken build** | Ada langkah yang gagal — prioritas utama untuk diperbaiki |
| **needs** | Dependency antar job — job B tidak jalan kalau job A belum selesai |
| **GHCR** | GitHub Container Registry — tempat menyimpan Docker image |
| **Multi-stage build** | Dockerfile dengan beberapa stage untuk menghasilkan image yang lebih kecil |
| **Registry** | Tempat menyimpan dan mendistribusikan Docker image |

---

## 9. Pelajaran & Gotchas

### Error yang ditemui dan solusinya

| Error | Penyebab | Solusi |
|---|---|---|
| `MSB1003: Specify a project or solution file` | `dotnet restore` jalan di root repo, `.sln` ada di subfolder | Tambah `defaults.run.working-directory: MathService` |
| Pipeline tidak muncul di tab Actions | File `ci.yml` ada di `MathService/.github/` bukan di root `.github/` | Pindahkan ke root repo: `.github/workflows/ci.yml` |
| Railway "Build failed" | Root Directory belum di-set | Set Root Directory ke `MathService` di Settings Railway |
| Job deploy "Waiting for runner" 15+ menit | Railway CLI di pipeline tidak stable | Ganti ke Railway native GitHub integration |

### Best practices yang sudah diterapkan
- `needs` antar job — Docker tidak jalan kalau test gagal
- `if: github.ref == 'refs/heads/main'` — image hanya di-push saat push ke main, bukan PR
- Multi-stage Dockerfile — image production lebih kecil
- `GITHUB_TOKEN` untuk auth ke GHCR — tidak perlu buat secret manual
- `working-directory` di level `defaults` — tidak perlu repeat di setiap step

---

## 10. Fase 5 — Next Steps

Yang belum dipelajari dan bisa dilanjutkan:

- [ ] **Caching NuGet dependencies** — supaya pipeline tidak download ulang setiap run, bisa hemat 1-2 menit per run
- [ ] **Parallel jobs** — test unit + linting + security scan jalan bersamaan
- [ ] **Security scanning** — Dependabot untuk scan vulnerability di dependencies
- [ ] **Reusable workflows** — satu template workflow untuk banyak repo (DRY principle)
- [ ] **Environment protection rules** — approval manual sebelum deploy ke production
- [ ] **Rollback strategy** — cara balik ke versi sebelumnya kalau deploy gagal
- [ ] **VPS + SSH deploy** — deploy ke server sendiri via SSH, lebih realistis untuk kerja

### Referensi berguna
- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [Docker Multi-stage Build](https://docs.docker.com/build/building/multi-stage/)
- [Railway Docs](https://docs.railway.app)
- [actions/setup-dotnet](https://github.com/actions/setup-dotnet)
- [docker/build-push-action](https://github.com/docker/build-push-action)

---

> **Catatan:** Pipeline ini sudah production-grade — alur build → test → containerize → deploy yang sama dipakai di perusahaan nyata. Yang membedakan hanya skala dan kompleksitas test coverage-nya.
