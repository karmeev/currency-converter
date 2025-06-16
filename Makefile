ROOT_DIR := $(shell git rev-parse --show-toplevel)
VERSION := $(shell cat $(ROOT_DIR)/VERSION.txt)

UNIT_TESTS=./tests/unit-tests
INTEGRATION_TESTS=./tests/integrations-tests
APP=infra/docker
APP_TEST=infra/docker-test

UNIT_TEST_PROJECTS := \
    Currency.Data.Tests/Currency.Data.Tests.csproj \
    Currency.Facades.Tests/Currency.Facades.Tests.csproj \
    Currency.Infrastructure.Tests/Currency.Infrastructure.Tests.csproj \
    Currency.Services.Tests/Currency.Services.Tests.csproj \
    Currency.Common.Tests/Currency.Common.Tests.csproj \

INTEGRATION_TEST_PROJECTS := \
    Currency.IntegrationTests.Infrastructure/Currency.IntegrationTests.Infrastructure.csproj \
    Currency.IntegrationTests.Api/Currency.IntegrationTests.Api.csproj \

currency:
	echo "APP_VERSION=$(VERSION)" > $(ROOT_DIR)/.env
	docker network inspect currency_network >/dev/null 2>&1 || docker network create currency_network && \
    docker compose --env-file .env -f ${APP}/docker-compose.worker.yaml up -d
	sleep 2s
	docker compose --env-file .env -f ${APP}/docker-compose.worker3.yaml up -d
	sleep 4s
	docker compose --env-file .env -f ${APP}/docker-compose.worker2.yaml up -d
	docker compose --env-file .env -f ${APP}/docker-compose.master.yaml up -d

stop:
	docker compose -f ${APP}/docker-compose.master.yaml down
	docker compose -f ${APP}/docker-compose.worker3.yaml down
	docker compose -f ${APP}/docker-compose.worker2.yaml down
	docker compose -f ${APP}/docker-compose.worker.yaml down

test:
	@if [ "$(CATEGORY)" = "Unit" ]; then \
		TEST_PATH=$(UNIT_TESTS); \
		PROJECTS="$(UNIT_TEST_PROJECTS)"; \
	elif [ "$(CATEGORY)" = "Integration" ]; then \
		TEST_PATH=$(INTEGRATION_TESTS); \
		PROJECTS="$(INTEGRATION_TEST_PROJECTS)"; \
	else \
		echo "Unsupported CATEGORY '$(CATEGORY)'. Use 'Unit' or 'Integration'."; \
		exit 1; \
	fi; \
	for proj in $$PROJECTS; do \
		echo "Running $(CATEGORY) tests in $$proj..."; \
		dotnet test $$TEST_PATH/$$proj \
			--configuration Release \
			--no-restore \
			--logger "console;verbosity=detailed" \
			--filter "Category=$(CATEGORY)"; \
	done

coverage:
	@for proj in $(UNIT_TEST_PROJECTS); do \
		echo "Running coverage for $$proj..."; \
		dotnet test $(UNIT_TESTS)/$$proj \
			--configuration Release \
			--collect:"XPlat Code Coverage" \
			--results-directory ./TestResults \
			--logger "trx;LogFileName=test-results.trx"; \
	done

	reportgenerator \
		-reports:./TestResults/**/coverage.cobertura.xml \
		-targetdir:./TestResults/CoverageReport \
		-reporttypes:MarkdownSummaryGithub
		-assemblyfilters:+Currency.*;-*.Contracts;-*Tests;-xunit*;-System.*;-Microsoft.*

integration_tests_up:
	echo "APP_VERSION=$(cat VERSION.txt)" > .env
	docker network inspect currency_network >/dev/null 2>&1 || docker network create currency_network && \
    docker compose --env-file .env -f ${APP_TEST}/docker-compose.worker.yaml up -d
	sleep 2s
	docker compose --env-file .env -f ${APP_TEST}/docker-compose.worker3.yaml up -d
	sleep 4s
	docker compose --env-file .env -f ${APP_TEST}/docker-compose.worker2.yaml up -d
	docker compose --env-file .env -f ${APP_TEST}/docker-compose.master.yaml up -d

integration_tests_down:
	docker compose -f ${APP_TEST}/docker-compose.master.yaml down
	docker compose -f ${APP_TEST}/docker-compose.worker3.yaml down
	docker compose -f ${APP_TEST}/docker-compose.worker2.yaml down
	docker compose -f ${APP_TEST}/docker-compose.worker.yaml down