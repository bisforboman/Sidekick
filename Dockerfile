FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
USER app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
RUN apk add nodejs npm
WORKDIR /src

COPY ./*.sln ./

ENV HOME=/src

# See https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files-part-2/#the-new-improved-solution
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done

RUN dotnet restore
COPY . .

RUN dotnet build -c Release

FROM build AS publish

RUN sed -i '/Sidekick.Wpf/,+1d' Sidekick.sln

RUN dotnet publish -c Release

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

WORKDIR /app/src/Sidekick.Web
VOLUME /app/src/Sidekick.Web/sidekick

ENTRYPOINT ["dotnet", "Sidekick.dll"]
