#!/bin/bash

dotnet test --filter TestCategory=Unit \
	/p:CollectCoverage=true \
	/p:CoverletOutput=../coverage/ \
	/p:CoverletOutputFormat=cobertura \
	/p:Exclude=\"[*]EqlibApi.Migrations.*,[EqlibApi.Program]\" \
	EqlibApi.Tests

reportgenerator "-reports:./coverage/coverage.cobertura.xml" \
	"-targetdir:./coverage" \
	"-sourcedirs:EqlibApi" \
	"-reporttypes:Html"
