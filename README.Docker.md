# Docker Deployment Guide - Hospital Management Microservices

Hướng dẫn triển khai hệ thống quản lý bệnh viện bằng Docker.

## Yêu cầu hệ thống

- **Docker Desktop** (Windows/Mac) hoặc **Docker Engine** (Linux)
- **Docker Compose** v2.0+
- RAM tối thiểu: 8GB (SQL Server yêu cầu 2GB)

## Cấu trúc Services

| Service | Port | Mô tả |
|---------|------|-------|
| `hospital-db` | 1433 | SQL Server 2022 Express |
| `identity-service` | 5283 | Xác thực & JWT |
| `admin-service` | 5270 | Quản lý user & audit logs |
| `bacsi-service` | 5163 | Quản lý bác sĩ |
| `benhnhan-service` | 5146 | Quản lý bệnh nhân |
| `yta-service` | 5259 | Quản lý y tá |
| `khoaphong-service` | 5178 | Quản lý khoa phòng |
| `hoadon-service` | 5231 | Quản lý hóa đơn |
| `api-gateway` | 5076 | Gateway (Ocelot) |

## Khởi động nhanh

### 1. Build và chạy tất cả services

```cmd
cd d:\project-webapi-backend\ApiGateway
docker-compose up -d --build
```

### 2. Kiểm tra trạng thái

```cmd
docker-compose ps
```

### 3. Xem logs

```cmd
docker-compose logs -f api-gateway
docker-compose logs -f hospital-db
```

### 4. Truy cập API

- **API Gateway**: http://localhost:5076
- **IdentityService Swagger**: http://localhost:5283/swagger
- **SQL Server**: localhost,1433 (User: sa, Pass: 123456)

## Các lệnh thường dùng

| Lệnh | Mô tả |
|------|-------|
| `docker-compose up -d` | Khởi động tất cả |
| `docker-compose down` | Dừng tất cả |
| `docker-compose down -v` | Dừng và xóa data |
| `docker-compose restart [service]` | Restart 1 service |
| `docker-compose logs -f [service]` | Xem logs |
| `docker-compose build --no-cache` | Build lại từ đầu |

## Khắc phục sự cố

### SQL Server không khởi động
```cmd
docker-compose logs hospital-db
```
Kiểm tra password SA có đủ mạnh không (tối thiểu 8 ký tự, có chữ hoa, chữ thường, số).

### Service không kết nối được database
Đợi 30-60 giây để SQL Server khởi động hoàn tất, sau đó restart service:
```cmd
docker-compose restart identity-service
```

### Xem tất cả logs
```cmd
docker-compose logs -f --tail=100
```
