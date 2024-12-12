# Demos
&emsp;&emsp;hangfire and hangfire.httpjob Demo
## 1. 安装
&emsp;&emsp;Hangfire.AspNetCore
&emsp;&emsp;Hangfire.HttpJob

## 2. 配置
&emsp;&emsp;appsettings.json

## docker 容器网络
> 1. 创建网络
``` 
docker network create my-network
```
> 2. 容器加入网络
```
docker run .... --network=my-network
```

## 容器间通信
使用容器名作为服务地址，例如mysql： 容器名：mysql , 服务地址：mysql

jobserver 容器名：jobserver, 服务地址：jobserver


