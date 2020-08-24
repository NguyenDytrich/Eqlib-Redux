FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
COPY . /src
WORKDIR /src/EqlibApi.Tests
RUN curl https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /src/wait_for_it.sh \
    && dotnet restore