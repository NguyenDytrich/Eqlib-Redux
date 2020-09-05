#!/bin/bash

dotnet test \
	--results-directory "./coverage" \
	/p:CollectCoverage=true \
	/p:CoverletOutput=../coverage/ \
	/p:CoverletOutputFormat=cobertura \
	/p:Exclude=\"[*]EqlibApi.Migrations.*,[EqlibApi.program\"

/tools/reportgenerator "-reports:/src/coverage/coverage.cobertura.xml" \
	"-targetdir:/src/coverage" \
	"-reporttypes:Html"
