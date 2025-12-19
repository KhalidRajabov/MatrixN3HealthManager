# Настройка OpenVPN для маршрутизации исходящего трафика

## Важно о маршрутизации в Kubernetes

В Kubernetes контейнеры в поде используют **общий network namespace**, но для маршрутизации трафика через VPN интерфейс из sidecar контейнера в основной контейнер требуется дополнительная настройка.

## Варианты реализации

### Вариант 1: Использование hostNetwork (простой, но менее безопасный)

Если использовать `hostNetwork: true`, поды будут использовать сеть узла напрямую, и VPN интерфейс будет доступен всем контейнерам.

**Плюсы:**
- Простая настройка
- VPN работает сразу

**Минусы:**
- Поды доступны по IP узла (менее безопасно)
- Могут быть конфликты портов

### Вариант 2: Использование сетевых политик и маршрутизации (рекомендуется)

Настроить маршрутизацию на уровне узла через CNI плагин или использовать сетевые политики.

## Инструкция по настройке

### Шаг 1: Подготовка OpenVPN конфигурации

1. Получите от администратора OpenVPN сервера:
   - Файл конфигурации клиента (`.ovpn`)
   - CA сертификат (`ca.crt`)
   - Клиентский сертификат (`client.crt`)
   - Клиентский ключ (`client.key`)

2. Обновите `openvpn-configmap.yaml`:
   - Замените `YOUR_VPN_SERVER_IP` на реальный IP адрес VPN сервера
   - Настройте параметры шифрования под ваш сервер

3. Создайте Secret с сертификатами:
   ```bash
   kubectl create secret generic openvpn-credentials \
     --from-file=ca.crt=./ca.crt \
     --from-file=client.crt=./client.crt \
     --from-file=client.key=./client.key \
     -n matrixcrm-prod
   ```

   Или отредактируйте `openvpn-secret-template.yaml` и примените:
   ```bash
   kubectl apply -f deploy/k8s/openvpn-secret-template.yaml
   ```

### Шаг 2: Применение конфигурации

```bash
# Создать ConfigMap с конфигурацией OpenVPN
kubectl apply -f deploy/k8s/openvpn-configmap.yaml

# Создать Secret с сертификатами (после заполнения)
kubectl apply -f deploy/k8s/openvpn-secret-template.yaml

# Обновить deployment
kubectl apply -f deploy/k8s/deployment.yaml
```

### Шаг 3: Проверка подключения

```bash
# Проверить логи OpenVPN контейнера
kubectl logs -n matrixcrm-prod -l app=medcard-n3health-backend -c openvpn-client

# Проверить VPN интерфейс
kubectl exec -n matrixcrm-prod -l app=medcard-n3health-backend -c openvpn-client -- ip addr show tun0

# Проверить маршруты
kubectl exec -n matrixcrm-prod -l app=medcard-n3health-backend -c openvpn-client -- ip route

# Проверить исходящий IP (должен быть VPN IP)
kubectl exec -n matrixcrm-prod -l app=medcard-n3health-backend -c medcard-n3health-backend -- curl -s ifconfig.me
```

## Альтернативный подход: Использование VPN Gateway на уровне кластера

Если нужна более надежная маршрутизация, рассмотрите использование:
- **Calico CNI** с VPN gateway
- **Cilium** с egress gateway
- **Network Policy** с VPN proxy на уровне узла

## Устранение проблем

### VPN не подключается
- Проверьте логи: `kubectl logs -n matrixcrm-prod -l app=medcard-n3health-backend -c openvpn-client`
- Убедитесь, что сертификаты корректны
- Проверьте доступность VPN сервера из кластера

### Трафик не идет через VPN
- Убедитесь, что в конфигурации есть `redirect-gateway def1`
- Проверьте маршруты: `ip route` в контейнере
- Возможно потребуется `hostNetwork: true` в deployment

### Конфликты портов
- Если используется `hostNetwork: true`, убедитесь что порт 8080 не занят на узле
- Рассмотрите использование NodePort или другого подхода








