# Dokumentasi Belajar CI/CD
> **Author:** Martin Minggu  
> **Repo:** [BelajarCi-CD](https://github.com/MartinMinggu/BelajarCi-CD)  
> **Stack:** .NET 8, GitHub Actions, Docker, GHCR, Railway  
> **Durasi:** Fase 1–5 (Minggu 1–8) — SELESAI ✅

---

## Daftar Isi
1. [Konsep Dasar CI/CD](#1-konsep-dasar-cicd)
2. [Tools yang Digunakan](#2-tools-yang-digunakan)
3. [Struktur Project](#3-struktur-project)
4. [Fase 2 — GitHub Actions Pipeline](#4-fase-2--github-actions-pipeline)
5. [Fase 3 — Docker dalam Pipeline](#5-fase-3--docker-dalam-pipeline)
6. [Fase 4 — Deploy Otomatis ke Railway](#6-fase-4--deploy-otomatis-ke-railway)
7. [Fase 5 — Optimasi & Best Practices](#7-fase-5--optimasi--best-practices)
8. [Alur Lengkap Pipeline Final](#8-alur-lengkap-pipeline-final)
9. [ci.yml Final — Pipeline Lengkap](#9-ciyml-final--pipeline-lengkap)
10. [Kosakata Penting](#10-kosakata-penting)
11. [Semua Gotchas & Solusinya](#11-semua-gotchas--solusinya)
12. [Catatan Tambahan](#12-catatan-tambahan)

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
| **Railway** | Platform cloud untuk deploy & hosting | Trial $5 / 30 hari |

### Kenapa GitHub Actions untuk pemula?
- Tidak perlu install apapun
- Tidak perlu daftar akun baru
- Cukup buat file `.github/workflows/ci.yml` di repo
- Untuk .NET sudah ada `actions/setup-dotnet` yang tinggal pakai

---

## 3. Struktur Project

```
BelajarCi-CD/                               ← root repository
├── .github/
│   ├── dependabot.yml                      ← security scanning (Fase 5)
│   └── workflows/
│       ├── ci.yml                          ← pipeline utama
│       └── dotnet-ci-template.yml          ← reusable workflow template (Fase 5)
└── MathService/                            ← project .NET
    ├── MathService.sln
    ├── Dockerfile
    ├── railway.toml
    ├── src/
    │   └── MathService/
    │       ├── MathService.csproj
    │       ├── Program.cs
    │       ├── Models/
    │       │   └── TodoItem.cs
    │       ├── Services/
    │       │   ├── MathLogic.cs
    │       │   ├── StringService.cs
    │       │   └── DateService.cs
    │       └── Endpoints/
    │           ├── MathEndpoints.cs
    │           ├── StringEndpoints.cs
    │           ├── DateEndpoints.cs
    │           └── TodoEndpoints.cs
    └── tests/
        └── MathService.Tests/
            ├── MathService.Tests.csproj
            ├── MathServiceTests.cs
            ├── StringServiceTests.cs
            └── DateServiceTests.cs
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

### Program.cs — Minimal API (versi final)

```csharp
using MathService.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "MathService API is running!");

app.MapMathEndpoints();
app.MapStringEndpoints();
app.MapDateEndpoints();
app.MapTodoEndpoints();

app.Run();

public partial class Program { }
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
| `needs:` | Dependency antar job — job B tunggu job A selesai |
| `if:` | Kondisi — job hanya jalan kalau kondisi ini terpenuhi |
| `workflow_call:` | Membuat workflow bisa dipanggil dari workflow lain |

### Catatan penting YAML
```yaml
# ❌ SALAH — YAML baca sebagai dua string terpisah, menyebabkan MSB1001
run: dotnet test tests/MathService.Tests
     --configuration Release

# ✅ BENAR — satu baris
run: dotnet test tests/MathService.Tests --configuration Release

# ✅ BENAR juga — pakai | untuk multi-line
run: |
  dotnet test tests/MathService.Tests \
    --configuration Release
```

---

## 5. Fase 3 — Docker dalam Pipeline

### Dockerfile (Multi-stage build)

```dockerfile
# Stage 1: Build — pakai SDK yang lengkap (~700MB)
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

# Stage 2: Runtime — hanya runtime (~200MB), jauh lebih kecil
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
Railway di-connect langsung ke GitHub repo — setiap push ke `main`, Railway otomatis rebuild dan redeploy. GitHub Actions tetap handle CI (test) dan packaging (Docker ke GHCR). Dua sistem bekerja paralel — ini pattern yang banyak dipakai di dunia nyata.

### Durasi deploy Railway (hasil nyata)
| Step | Waktu |
|---|---|
| Initialization | 6 detik |
| Build | 6 detik |
| Deploy | 5 detik |
| Network | 1 detik |
| **Total** | **~18 detik** |

---

## 7. Fase 5 — Optimasi & Best Practices

### 7.1 Caching NuGet Dependencies

Tanpa cache: `dotnet restore` download semua packages setiap run (~90 detik).
Dengan cache: packages diambil dari cache kalau tidak ada perubahan (~10 detik).

```yaml
- name: Cache NuGet packages
  uses: actions/cache@v4
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/MathService/**/*.csproj') }}
    restore-keys: |
      ${{ runner.os }}-nuget-
```

**Cara kerja cache key:**
- `hashFiles('**/*.csproj')` — hash dari semua file `.csproj`
- Kalau tidak ada dependency baru → hash sama → **cache hit** → skip download
- Kalau ada package baru ditambah → hash berubah → **cache miss** → download ulang, simpan cache baru
- `restore-keys` — fallback kalau exact key tidak cocok, pakai cache yang paling mirip

### 7.2 Parallel Jobs

Mengubah test dari serial menjadi paralel — tiga job test jalan bersamaan di mesin berbeda.

**Perbandingan durasi:**
| Mode | Durasi test |
|---|---|
| Serial (sebelum) | ~90 detik (30+30+30) |
| Parallel (sesudah) | ~30 detik (secepat job terlama) |

**Struktur parallel jobs:**
```
build (21s)
   ├── test-math   (~28s) ─┐
   ├── test-string (~26s) ─┼─ jalan bersamaan
   └── test-date   (~29s) ─┘
          ↓ semua lulus
       docker (~45s)
```

**Key concept — `needs` array:**
```yaml
docker:
  needs: [test-math, test-string, test-date]  # tunggu SEMUA lulus
```

**Filter test per job:**
```bash
# Hanya jalankan test dari class tertentu
dotnet test tests/MathService.Tests --filter "FullyQualifiedName~MathServiceTests"
dotnet test tests/MathService.Tests --filter "FullyQualifiedName~StringServiceTests"
dotnet test tests/MathService.Tests --filter "FullyQualifiedName~DateServiceTests"
```

### 7.3 Security Scanning — Dependabot

File `.github/dependabot.yml`:

```yaml
version: 2

updates:
  # Scan NuGet packages (.NET)
  - package-ecosystem: "nuget"
    directory: "/MathService"
    schedule:
      interval: "weekly"
      day: "monday"
      time: "09:00"
    open-pull-requests-limit: 5
    labels:
      - "dependencies"
      - "dotnet"

  # Scan GitHub Actions sendiri
  - package-ecosystem: "github-actions"
    directory: "/"
    schedule:
      interval: "weekly"
      day: "monday"
      time: "09:00"
    labels:
      - "dependencies"
      - "github-actions"
```

Setelah file ini di-push, setiap Senin pagi GitHub otomatis scan semua dependencies dan buat PR kalau ada CVE atau versi outdated.

### 7.4 Reusable Workflows

File `.github/workflows/dotnet-ci-template.yml`:

```yaml
name: .NET CI Template

on:
  workflow_call:
    inputs:
      dotnet-version:
        required: false
        type: string
        default: '8.0.x'
      working-directory:
        required: false
        type: string
        default: '.'

jobs:
  build:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: ${{ inputs.working-directory }}
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ inputs.dotnet-version }}
      - name: Cache NuGet
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
          restore-keys: ${{ runner.os }}-nuget-
      - run: dotnet restore
      - run: dotnet build --no-restore --configuration Release
      - run: dotnet test --no-build --configuration Release
```

**Cara pakai dari repo lain:**
```yaml
jobs:
  ci:
    uses: MartinMinggu/BelajarCi-CD/.github/workflows/dotnet-ci-template.yml@main
    with:
      dotnet-version: '8.0.x'
      working-directory: 'NamaProjectKamu'
```

---

## 8. Alur Lengkap Pipeline Final

```
Developer: git push ke main
           ↓
GitHub Actions — Job: build
  - checkout kode
  - setup .NET 8
  - cache NuGet (hit/miss)
  - dotnet restore
  - dotnet build --configuration Release
           ↓
           ├── Job: test-math   ─┐
           ├── Job: test-string ─┼─ paralel, masing-masing ~28s
           └── Job: test-date   ─┘
                    ↓ semua lulus
           Job: docker
  - login ke ghcr.io
  - docker build (multi-stage)
  - docker push → ghcr.io/martinminggu/mathservice:latest
           ↓
Railway (paralel, native integration)
  - detect push ke main
  - build dari Dockerfile
  - deploy container baru (~18 detik)
           ↓
https://xxx.railway.app — langsung update!
```

### Kondisi proteksi pipeline
| Kondisi | Efek |
|---|---|
| Build gagal | Test dan Docker tidak jalan |
| Satu test job gagal | Docker tidak jalan |
| Event adalah PR (bukan push) | Docker tidak push image ke registry |
| Push bukan ke `main` | Docker tidak push image ke registry |

---

## 9. ci.yml Final — Pipeline Lengkap

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    defaults:
      run:
        working-directory: MathService

    steps:
      - name: Checkout kode
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/MathService/**/*.csproj') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

  test-math:
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Checkout kode
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/MathService/**/*.csproj') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Run MathService tests
        working-directory: MathService
        run: dotnet test tests/MathService.Tests --configuration Release --verbosity normal --filter "FullyQualifiedName~MathServiceTests"

  test-string:
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Checkout kode
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/MathService/**/*.csproj') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Run StringService tests
        working-directory: MathService
        run: dotnet test tests/MathService.Tests --configuration Release --verbosity normal --filter "FullyQualifiedName~StringServiceTests"

  test-date:
    runs-on: ubuntu-latest
    needs: build
    steps:
      - name: Checkout kode
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/MathService/**/*.csproj') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Run DateService tests
        working-directory: MathService
        run: dotnet test tests/MathService.Tests --configuration Release --verbosity normal --filter "FullyQualifiedName~DateServiceTests"

  docker:
    runs-on: ubuntu-latest
    needs: [test-math, test-string, test-date]
    if: github.ref == 'refs/heads/main' && github.event_name == 'push'

    steps:
      - name: Checkout kode
        uses: actions/checkout@v4

      - name: Login ke GitHub Container Registry
        uses: docker/login-action@v3
        with:
          registry: ghcr.io
          username: ${{ github.actor }}
          password: ${{ secrets.GITHUB_TOKEN }}

      - name: Build dan push Docker image
        uses: docker/build-push-action@v5
        with:
          context: ./MathService
          push: true
          tags: ghcr.io/${{ github.repository_owner }}/mathservice:latest
```

---

## 10. Kosakata Penting

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
| **Multi-stage build** | Dockerfile dengan beberapa stage untuk image yang lebih kecil |
| **Registry** | Tempat menyimpan dan mendistribusikan Docker image |
| **Cache hit** | Data cache ditemukan dan langsung dipakai, tidak perlu download |
| **Cache miss** | Data cache tidak ditemukan, proses berjalan normal lalu disimpan |
| **Parallel jobs** | Beberapa job jalan bersamaan di mesin berbeda |
| **Reusable workflow** | Template workflow yang bisa dipanggil dari repo lain |
| **Dependabot** | Bot GitHub yang otomatis scan dan update dependency |
| **CVE** | Common Vulnerabilities and Exposures — celah keamanan yang terdokumentasi |
| **workflow_call** | Trigger yang membuat workflow bisa dipanggil workflow lain |

---

## 11. Semua Gotchas & Solusinya

| Error / Masalah | Penyebab | Solusi |
|---|---|---|
| `MSB1003: Specify a project or solution file` | `dotnet restore` jalan di root repo, `.sln` ada di subfolder | Tambah `defaults.run.working-directory: MathService` |
| Pipeline tidak muncul di tab Actions | File `ci.yml` ada di `MathService/.github/` bukan di root `.github/` | Pindahkan ke root repo: `.github/workflows/ci.yml` |
| Railway "Build failed" | Root Directory belum di-set | Set Root Directory ke `MathService` di Settings Railway |
| Job deploy "Waiting for runner" 15+ menit | Railway CLI di pipeline tidak stabil | Ganti ke Railway native GitHub integration |
| `MSB1001: Unknown switch --project` | `run:` ditulis multi-line di YAML tanpa `\|` — dibaca sebagai dua argumen terpisah | Tulis dalam satu baris atau gunakan `\|` untuk multi-line shell |
| Cache tidak aktif / tidak hemat waktu | `hashFiles` path salah — tidak menemukan file `.csproj` | Pastikan path di `hashFiles` sesuai struktur folder project |
| Docker job jalan padahal test gagal | `needs` tidak diisi dengan benar | Pastikan `needs: [test-math, test-string, test-date]` di job docker |
| Parallel test job semua gagal padahal lokal lulus | `dotnet test` butuh `dotnet restore` dulu di runner baru | Pastikan setiap job test punya step cache + restore sebelum test |

---

## 12. Catatan Tambahan

### Minimal API vs Controller
Project ini menggunakan **Minimal API** (.NET 6+) bukan Controller MVC tradisional.

| Aspek | Controller | Minimal API |
|---|---|---|
| Paradigma | OOP — class dengan method | Functional — lambda/delegate |
| Routing | Attribute `[Route]`, `[HttpGet]` | `MapGet()`, `MapPost()` langsung |
| Boilerplate | Lebih banyak | Sangat minimal |
| Performance | Sedikit lebih lambat | Lebih cepat |
| Cocok untuk | API besar, tim besar | Microservice, API kecil-menengah |

### Middleware & Security di Minimal API
Minimal API support penuh middleware dan security — caranya sedikit berbeda dari Controller:

```csharp
// Urutan middleware SANGAT penting
app.UseHttpsRedirection();
app.UseAuthentication();    // harus sebelum UseAuthorization
app.UseAuthorization();
app.UseRateLimiter();
app.MapTodoEndpoints();     // endpoint didaftarkan SETELAH middleware

// Auth di endpoint
group.MapDelete("/{id}", DeleteTodo).RequireAuthorization();
group.MapPost("/", CreateTodo).RequireRateLimiting("fixed");
group.MapPost("/", CreateTodo).AddEndpointFilter<ValidationFilter>();
```

### Railway Pricing (2026)
| Plan | Harga | Keterangan |
|---|---|---|
| Trial | Gratis | $5 credit, 30 hari, tanpa kartu kredit |
| Free | $1/bulan | Setelah trial, app tetap hidup minimal |
| Hobby | $5/bulan | $5 credit included, untuk project serius |

### Referensi
- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [Docker Multi-stage Build](https://docs.docker.com/build/building/multi-stage/)
- [Railway Docs](https://docs.railway.app)
- [actions/setup-dotnet](https://github.com/actions/setup-dotnet)
- [docker/build-push-action](https://github.com/docker/build-push-action)
- [actions/cache](https://github.com/actions/cache)
- [Dependabot Docs](https://docs.github.com/en/code-security/dependabot)

### Next Steps (kalau mau lanjut DevOps)
- [ ] **Kubernetes** — orkestrasi container untuk skala besar
- [ ] **Monitoring** — Prometheus + Grafana untuk observability
- [ ] **Environment gates** — approval manual sebelum deploy ke production
- [ ] **Rollback strategy** — cara balik ke versi sebelumnya otomatis
- [ ] **VPS + SSH deploy** — deploy ke server sendiri, lebih realistis untuk kerja
- [ ] **GitOps** — ArgoCD atau Flux untuk deployment berbasis Git

---

> Pipeline yang dibangun di dokumentasi ini sudah **production-grade** — alur build → test paralel → containerize → deploy otomatis yang sama dipakai di perusahaan nyata. Yang membedakan hanya skala, jumlah environment, dan kompleksitas test coverage-nya.
