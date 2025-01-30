# tdm-prototype-scheduler

### Development image

Build:

```bash
docker build --tag tdm-prototype-scheduler .
```

Run:

```bash
docker run -p 8080:8080 tdm-prototype-scheduler
```

Run, passing an API to connect to:

```bash
docker run -p 8080:8080 -e TDM_API=https://localhost:7094/ tdm-prototype-scheduler
```

Connect & poke around:

```bash
docker run --rm -it -p 8080:8080 --entrypoint ash tdm-prototype-scheduler
```