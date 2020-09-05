FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

# Copy bare necessities
COPY EqlibApi /src/EqlibApi
COPY /EqlibApi.Tests /src/EqlibApi.Tests
COPY Eqlib.sln /src
COPY scripts/run_test_suite.sh /src
RUN chmod +x /src/run_test_suite.sh

# Install report generator
RUN dotnet tool install dotnet-reportgenerator-globaltool --tool-path /tools
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /src/wait_for_it.sh

LABEL testcontainer

WORKDIR /src/EqlibApi.Tests
RUN dotnet restore && dotnet build
