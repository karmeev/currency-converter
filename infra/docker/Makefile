currency:
	docker network inspect currency_network >/dev/null 2>&1 || docker network create currency_network && \
    docker compose -f docker-compose.worker.yaml up -d
	sleep 2s
	docker compose -f docker-compose.worker3.yaml up -d
	sleep 4s
	docker compose -f docker-compose.worker2.yaml up -d
	docker compose up -d

stop:
	docker compose down
	docker compose -f docker-compose.worker3.yaml down
	docker compose -f docker-compose.worker2.yaml down
	docker compose -f docker-compose.worker.yaml down