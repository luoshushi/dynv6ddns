# dynv6ddns 
## netcore 版本动态dns映射 跨平台
- docker 使用
- git clone https://github.com/luoshushi/dynv6ddns.git
-  cd dynv6ddns/dynv6ddns
-  docker build -t dynv6 .
-  docker run -itd --name dynv6 -e token="xxx" -e hostname="xxx.dynv6.net" dynv6
