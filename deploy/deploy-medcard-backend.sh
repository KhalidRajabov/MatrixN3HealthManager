#!/usr/bin/env bash
set -euo pipefail

NAMESPACE="matrixcrm-prod"
APP_LABEL="medcard-n3health-backend"
IMAGE="cr.yandex/crpbaq5rr0b32nc3630p/matrix-n3-health-manager:latest"

echo "=== 1/4: Build docker image (${IMAGE}) ==="
docker buildx build \
  --platform linux/amd64 \
  -t "${IMAGE}" .

echo "=== 2/4: Push docker image ==="
docker push "${IMAGE}"

echo "=== 3/4: Apply k8s manifests (VPN, deployment, service, ingress) ==="
kubectl apply -f deploy/k8s/openvpn-secret-template.yaml || true
kubectl apply -f deploy/k8s/openvpn-configmap.yaml
kubectl apply -f deploy/k8s/deployment.yaml
kubectl apply -f deploy/k8s/service.yaml
kubectl apply -f deploy/k8s/ingress.yaml

echo "=== 4/4: Restart pods for ${APP_LABEL} in namespace ${NAMESPACE} ==="
kubectl delete pod -n "${NAMESPACE}" -l app="${APP_LABEL}" || true

echo "Waiting for pod to become Running..."
for i in {1..30}; do
  POD=$(kubectl get pods -n "${NAMESPACE}" -l app="${APP_LABEL}" --field-selector=status.phase=Running -o jsonpath='{.items[0].metadata.name}' 2>/dev/null || true)
  if [ -n "${POD}" ]; then
    echo "Pod is running: ${POD}"
    echo "Last 20 lines of VPN logs:"
    kubectl logs -n "${NAMESPACE}" "${POD}" -c openvpn-client --tail=20 || true
    exit 0
  fi
  sleep 5
done

echo "WARNING: Pod is not Running yet. Check pods manually:"
echo "kubectl get pods -n ${NAMESPACE} -l app=${APP_LABEL}"
exit 1




