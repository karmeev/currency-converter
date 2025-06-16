# Explore API

---

### :memo: Step 0. Postman
First, you need to download the postman collection. It contains endpoints for further interaction with the API.
The repository contains two versions of collections according to Postman. You can choose either one.

### :lock: Step 1. Login & Refresh Token
To make a request to protected endpoints, you need to get a token. To do this, you need to make a login request, 
which returns an access token.
If the protected endpoints returns 401, you need to make a refresh token request, which will provide a new access token.
> [!TIP]
> Postman automatically sets an access token for login and refresh token requests

### :fire: Step 2. Interact (Enjoy)
Now you can use the API...

> [!IMPORTANT]  
> Some endpoints have RBAC restrictions. To access them, log in as an admin

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