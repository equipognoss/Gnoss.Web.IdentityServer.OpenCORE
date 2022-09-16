![](https://content.gnoss.ws/imagenes/proyectos/personalizacion/7e72bf14-28b9-4beb-82f8-e32a3b49d9d3/cms/logognossazulprincipal.png)

# Gnoss.Web.IdentityServer.OpenCORE

![](https://github.com/equipognoss/Gnoss.Web.IdentityServer.OpenCORE/workflows/BuildIdentityServer/badge.svg)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=bugs)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=equipognoss_Gnoss.Web.IdentityServer.OpenCORE&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=equipognoss_Gnoss.Web.IdentityServer.OpenCORE)

Aplicación Web que se encarga de proveer y validar los tokens de acceso para acceder a las aplicaciones de uso interno (intern, ontologies y documents). 

Configuración estandar de esta aplicación en el archivo docker-compose.yml: 

```yml
identityserver:
    image: gnoss/gnoss.web.identityserver.opencore
    env_file: .env
    ports:
     - ${puerto_identity}:80
    environment:
     virtuosoConnectionString: ${virtuosoConnectionString}
     IssuerUri: ${IssuerUri}
     scopeIdentity: ${scopeIdentity}
     clientSecretIdentity: ${clientSecretIdentity}
     clientIDIdentity: ${clientIDIdentity}
     segundostoken: ${segundostoken}
    volumes:
      - ./logs/identityserver:/app/logs
```

Se pueden consultar los posibles valores de configuración de cada parámetro aquí: https://github.com/equipognoss/Gnoss.SemanticAIPlatform.OpenCORE

## Código de conducta
Este proyecto a adoptado el código de conducta definido por "Contributor Covenant" para definir el comportamiento esperado en las contribuciones a este proyecto. Para más información ver https://www.contributor-covenant.org/

## Licencia
Este producto es parte de la plataforma [Gnoss Semantic AI Platform Open Core](https://github.com/equipognoss/Gnoss.SemanticAIPlatform.OpenCORE), es un producto open source y está licenciado bajo GPLv3.
