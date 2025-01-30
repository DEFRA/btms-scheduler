# btms-scheduler

### Development image

Build:

```bash
docker build --tag btms-scheduler .
```

Run:

```bash
docker run --rm --name btms-scheduler -p 8080:8080 btms-scheduler
```

Run, passing an API to connect to:

```bash
docker run --rm --name btms-scheduler -p 8080:8080 -e CORE_BACKEND_API_URL=http://btms-backend.localtest.me:5002/ btms-scheduler
```

Connect & poke around:

```bash
docker run --name btms-scheduler --rm -it -p 8080:8080 --entrypoint ash btms-scheduler
```