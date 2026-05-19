.PHONY: github docker-build

docker-build:
	@if [ -z "$(NUGET_GITHUB_CREDENTIALS)" ]; then \
		echo "Usage: make docker-build NUGET_GITHUB_CREDENTIALS='Username=<github-user>;Password=<github-token>'"; \
		exit 1; \
	fi
	docker build \
		--build-arg NUGET_GITHUB_CREDENTIALS="$(NUGET_GITHUB_CREDENTIALS)" \
		-t stay-saga-user-service:latest .

github:
	@if [ -z "$(CM)" ]; then \
		echo "Usage: make github CM=\"commit message\""; \
		exit 1; \
	fi
	git add .
	git commit -m "$(CM)"
	git push origin main
