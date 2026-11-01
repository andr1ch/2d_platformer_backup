Лабораторная 3

Шаг 1: Настройка сжатия и пересборка WebGL

Включено сжатие Gzip, Decompression Fallback отключен. После пересборки получены файлы .framework.js.gz, .wasm.gz и .data.gz.

Шаг 2: Настройка Nginx

В [nginx.conf](nginx.conf) добавлены Content-Encoding: gzip и нужные Content-Type для JavaScript, WebAssembly и данных игры.

Шаг 3: Создание Dockerfile

В [Dockerfile](Dockerfile) использован nginx:alpine. Конфигурация Nginx заменена, сборка игры скопирована в /usr/share/nginx/html/.

Шаг 4: Локальная проверка

```powershell
docker build -t platformer-compressed:v1 .
docker run -d -p 8080:80 --name unity_compressed_container platformer-compressed:v1
```

Игра запущена на http://localhost:8080. В DevTools у WebGL.wasm.gz проверены 200 OK, Content-Encoding: gzip и Content-Type: application/wasm.

![](Screenshots/LR3/04-local-devtools.png)

Шаг 5: Обновление GitHub Actions

В [пайплайн](.github/workflows/main.yaml) добавлена задача build_docker_image: проверка Nginx и сборка образа с публикацией в GHCR. В облаке используются временные Gzip-заглушки, как предусмотрено методичкой.

Шаг 6: Публикация образа

Изменения отправлены в GitHub. Образ ghcr.io/andr1ch/platformer-game опубликован с тегами latest и SHA коммита.

Шаг 7: Проверка результатов

[Все три задачи завершились успешно](https://github.com/andr1ch/ARPO_LAB1/actions/runs/41344578516): sanity_check, mirror_repo и build_docker_image.

![](Screenshots/LR3/07-actions-success.png)

Контрольные вопросы

1. Content-Encoding: gzip указывает браузеру распаковать ответ сервера.
2. Заглушки позволяют собрать Docker-образ в CI до подключения облачной сборки Unity.
3. Файлы игры размещаются в /usr/share/nginx/html/.
4. latest обозначает последнюю версию, SHA фиксирует конкретную сборку для отката.
