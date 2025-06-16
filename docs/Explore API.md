# Explore API

---

### :memo: Step 0: Postman Setup
Start by downloading the [Postman collection](./../resources) provided in the repository. It includes all available endpoints for interacting with the API.
Two collection formats are included—feel free to use the one that suits your Postman version.

### :lock: Step 1: Authentication
To access protected endpoints, you'll first need to authenticate via the login endpoint, which returns an access token.
If you receive a `401 Unauthorized` response, use the refresh token endpoint to obtain a new access token.
> [!TIP]
> Postman will automatically apply the latest access token from your login or refresh requests.

### :fire: Step 2: Use the API
You're now ready to explore and interact with the API!

> [!IMPORTANT]  
> Some endpoints are restricted by RBAC (Role-Based Access Control). Admin login is required to access them.

#### Test Credentials:

Admin:
```
log: test-admin
pass: my_test_password_2
```

User:
```
log: test-user
pass: my_test_password
```