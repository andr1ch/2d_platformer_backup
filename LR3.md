Лабораторная 3

Шаг 1: Настройка Gzip и пересборка WebGL-проекта

В Unity 6 выбран Compression Format — Gzip, Decompression Fallback отключен. Пересборка выполнена через Unity CLI вызовом существующего BuildManager.BuildWebGL(). В Builds/WebGL/Build созданы WebGL.framework.js.gz, WebGL.wasm.gz и WebGL.data.gz.

Шаг 2: Создание конфигурации веб-сервера nginx.conf

Добавлены правила Content-Encoding: gzip и типы application/javascript, application/wasm и application/octet-stream. Отсутствующие архивы возвращают 404.

Шаг 3: Создание Dockerfile

Использован nginx:alpine. Стандартная конфигурация заменена на nginx.conf, файлы игры копируются в /usr/share/nginx/html/. Контейнер слушает порт 80. В .dockerignore оставлены только необходимые файлы сборки.

Шаг 4: Локальное тестирование контейнера

Локальная проверка пока заблокирована: Docker Desktop завершается с ошибкой служебного сокета docker-secrets-engine/engine.sock. Успешный запуск контейнера и заголовки браузера пока не подтверждены.

Команды после восстановления Docker Desktop:

```powershell
docker build -t platformer-compressed:v1 .
docker run -d -p 8080:80 --name unity_compressed_container platformer-compressed:v1
```

Открыть http://localhost:8080, затем F12 → Network → WebGL.wasm.gz → Headers. Проверить Content-Encoding: gzip и Content-Type: application/wasm.

```powershell
docker stop unity_compressed_container
docker rm unity_compressed_container
```

Шаг 5: Модернизация облачного пайплайна

В существующий .github/workflows/main.yaml добавлена третья задача build_docker_image. Задачи sanity_check и mirror_repo сохранены. Для публикации предоставлено packages: write, вход в GHCR выполняется через системный GITHUB_TOKEN.

В облаке временно создаются Gzip-заглушки, поскольку Builds/ исключена из Git. Облачный образ этой лабораторной проверяет доставку контейнера и не содержит скомпилированную игру. Локальный образ собирается с настоящей WebGL-игрой.

Шаг 6: Отправка изменений в GitHub

Образ настроен на публикацию с тегами ghcr.io/andr1ch/platformer-game:latest и ghcr.io/andr1ch/platformer-game:<github.sha>.

Шаг 7: Проверка результатов и оформление отчета

Отчеты первых двух лабораторных перенесены из README.md в LR1.md и LR2.md с сохранением скриншотов. В README.md добавлена таблица ссылок на три работы.

Контрольные вопросы

1. Content-Encoding: gzip сообщает браузеру, что тело ответа нужно распаковать перед выполнением JavaScript или WebAssembly. Тип содержимого задается отдельно через Content-Type.
2. Заглушки нужны, чтобы COPY Builds/WebGL/ в Dockerfile работала на GitHub Actions до подключения облачной сборки Unity. Здесь вместо пустых файлов созданы корректные Gzip-архивы.
3. Файлы игры копируются в /usr/share/nginx/html/.
4. latest указывает на последнюю опубликованную версию, а тег SHA фиксирует конкретную ревизию для воспроизводимости и отката.
