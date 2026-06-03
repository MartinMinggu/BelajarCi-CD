Pipeline : Rangkain langkah otomatis yang jalan setelah push
JOb : satu kelompok langkah dalam pipeline (misal job "build" , job "test")
step / stage : satu perintah tunggal dalam sebuah job
Runner / agent : mesin virtual yang menjalankan pipeline kamu
artifact : file hasil build (dll. , .exe, docker image) yang diteruskan antar  job
trigger : kondisi yang memicu pipeline jalan (push, pull, request, jadwal)
green build : pipeline selesai tanpa ada yang gagal 
broken build : ada langkah yang aggal - prioritas utaam untuk di perbaiki
apa itu CI/CD :
CI : Continous Integration: membangun / build dan menguji /test : setiap developer menuis kode baru atau memeperbaiki bug kode tersebut akan otomatis digabungkan ke dalam repository utama yang digunakan bersama dengan tim lain, setelah di gabungkan sistem CI akan secara otomatis menjalankan serangkain tes untuk memerikssa apakah kode baru tersebut berfungsi dengan baik dan tidak merusak fitur yang sudah ada
CD: Continuous Deployment : merilis / releasse dan mendistibusikan / deploy kode ke production (pengguna akhir): tahap lanjutan dari CI, jika di tahap CI tidak ada masalah: 