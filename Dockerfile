FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

RUN sed -i "s|MinProtocol = TLSv1.2|MinProtocol = TLSv1|g" /etc/ssl/openssl.cnf && \
    sed -i 's|CipherString = DEFAULT@SECLEVEL=2|CipherString = DEFAULT@SECLEVEL=1|g' /etc/ssl/openssl.cnf

RUN apt-get update && apt-get install -y --no-install-recommends curl

WORKDIR /app

COPY Gnoss.Web.IdentityServer/*.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet publish Gnoss.Web.IdentityServer/Gnoss.Web.IdentityServer.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN sed -i "s|MinProtocol = TLSv1.2|MinProtocol = TLSv1|g" /etc/ssl/openssl.cnf && \
    sed -i 's|CipherString = DEFAULT@SECLEVEL=2|CipherString = DEFAULT@SECLEVEL=1|g' /etc/ssl/openssl.cnf

RUN apt-get update && apt-get install -y --no-install-recommends curl

WORKDIR /app
RUN groupadd -g 2000 gnoss && useradd -u 2000 -g 2000 gnoss &&\
	mkdir -p logs trazas &&\
	chown -R gnoss:gnoss logs trazas && chmod -R 777 logs trazas
USER gnoss

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Gnoss.Web.IdentityServer.dll"]
