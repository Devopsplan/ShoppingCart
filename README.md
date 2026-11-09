# ShoppingCart

ShoppingCart is a sample online store. Customers can find products, add them to a cart, place orders, and track deliveries. Staff accounts can manage store orders and delivery locations.

## What it can do

- Browse and filter products, then view product details.
- Register, sign in, manage a profile, and use a wallet.
- Add products to a cart, place orders, and view order history.
- Track deliveries and manage store or delivery roles.
- Use the site on desktop and mobile, with light and dark themes.

## Built with

- **Frontend:** Angular 15 and Angular Material
- **Backend:** ASP.NET Core 7 Web API
- **Database:** PostgreSQL
- **Images:** Cloudinary (optional)

## Run with Docker

You need Git and Docker Compose. The app expects configuration values from a `.env` file in the project root. Add these variables and replace the example values with your own local development values:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=change-this-local-password
POSTGRES_DB=shoppingcart
ENVIRONMENT=Development
TokenKey=replace-with-a-long-random-development-key
AdminPassword=ChangeThis1!LocalPassword
DefaultConnection=Host=database;Port=5432;Database=shoppingcart;Username=postgres;Password=change-this-local-password
```

Keep real passwords and keys out of source control. The database username and password in `DefaultConnection` should match `POSTGRES_USER` and `POSTGRES_PASSWORD`. To enable profile photo uploads, also set `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, and `CLOUDINARY_API_SECRET` in `.env` using credentials from your Cloudinary account.

Clone the repository, create the `.env` file, then start the app:

```sh
git clone https://github.com/haseenavleturi/ShoppingCart.git
cd ShoppingCart
# Create .env using the values above.
docker compose up --build
```

Open [http://localhost:5000](http://localhost:5000).

PostgreSQL data is stored in the Compose-managed `postgres-data` volume and survives container recreation and `docker compose down`. The image is pinned to PostgreSQL 18 to prevent accidental major-version changes; use PostgreSQL's upgrade process before changing the major version. Do not run `docker compose down --volumes` (or `docker compose down -v`) unless you intend to delete the database.

## Deploy to Kubernetes

The manifests in `k8s/` deploy the app and PostgreSQL. They require a cluster with a default StorageClass and a Kubernetes Secret named `shoppingcart-secrets`.

1. Build the image and make it available to your cluster. For Minikube:

   ```sh
   docker build -t shoppingcart:latest .
   minikube image load shoppingcart:latest
   ```

   For a remote cluster, push the image to a container registry, then change `image` in `k8s/shoppingcart.yaml` to that registry image. Configure `imagePullSecrets` if the registry is private.

2. Create a protected environment file outside the repository, for example `/secure/path/shoppingcart-k8s.env`, with these keys. Use matching database credentials in `POSTGRES_PASSWORD` and `DefaultConnection`. Cloudinary keys are optional; add them to enable profile photo uploads.

   ```env
   POSTGRES_USER=postgres
   POSTGRES_PASSWORD=replace-with-a-strong-password
   POSTGRES_DB=ShoppingCart
   DefaultConnection=Host=shoppingcart-db;Port=5432;Database=ShoppingCart;Username=postgres;Password=replace-with-a-strong-password
   TokenKey=replace-with-a-long-random-signing-key
   AdminPassword=replace-with-a-strong-admin-password
   CLOUDINARY_CLOUD_NAME=your-cloud-name
   CLOUDINARY_API_KEY=your-api-key
   CLOUDINARY_API_SECRET=your-api-secret
   ```

   Create the Kubernetes Secret from that file; do not commit it:

   ```sh
   kubectl create secret generic shoppingcart-secrets \
     --from-env-file=/secure/path/shoppingcart-k8s.env
   ```

3. Apply the manifests and wait for the pods:

   ```sh
   kubectl apply -f k8s/postgres.yaml -f k8s/shoppingcart.yaml
   kubectl get pods
   ```

4. Wait for Kubernetes to assign an external address to the load balancer:

   ```sh
   kubectl get service shoppingcart --watch
   ```

   Open `http://<EXTERNAL-IP>:5000` once `EXTERNAL-IP` is assigned. Some local clusters (including Minikube) do not provide a load balancer by default; enable a load-balancer add-on such as MetalLB, or use `minikube tunnel` in a separate terminal.

PostgreSQL uses a StatefulSet and persistent volume claim. Keep the claim to retain database data; deleting it permanently deletes the database. The requested 5 GiB volume requires a compatible storage provisioner in the cluster.

## Run the unit tests

### Frontend

You need Node.js 16 and Chrome or Chromium available for Karma.

```sh
cd client
npm ci --legacy-peer-deps
npm run test:ci
```

### Backend

You need the .NET 7 SDK. From the project root, run:

```sh
dotnet test ShoppingCart.sln
```

Both test suites also run in GitHub Actions before the Docker image is built.
