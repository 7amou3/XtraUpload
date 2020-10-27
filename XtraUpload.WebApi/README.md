To authenticate the storage server(s) a tls certificate is needed, to simplify the process we are going to self sign the certificates that must be deployed in each storage server.
the first step is to designate a private CA (certificate authority) server, in our case it's going to be the server that host the XtraUpload API.
### Create a CA
1. Open ssl terminal and create a key
`openssl genrsa -out rootCA.key 2048` 
**Optional:** you can specify a password or enhance key strenght ex: 
`openssl genrsa -des3 -out rootCA.key 4096`
**PS:** this is the key used to sign the certificate requests, anyone holding this can sign certificates on your behalf. So keep it in a safe place!
2. Create and self sign the Root Certificate
`openssl req -x509 -new -nodes -key rootCA.key -sha256 -days 3650 -out rootCA.crt`
3. Host this certificate on your server's trusted root certificate store, for windows follow this [steps](https://www.thesslstore.com/knowledgebase/ssl-install/how-to-import-intermediate-root-certificates-using-mmc/). The server is ready to validate incoming http requests
### Create a client certificate (must be done for each storage server)
This procedure needs to be followed for each storage server that needs a trusted certificate from our CA (the XtraUpload API server), we will assume that the storage server is hosted on https://localhost:5002/
1. On the same terminal, create a certificate key
`openssl genrsa -out localhost.5002.key 2048`
2. Create the signing request (csr)
`openssl req -new -sha256 -key localhost.5002.key -subj "/C=US/ST=CA/O=XtraUpload, Inc./CN=localhost:5002" -out localhost.5002.csr`
3. Verify csr content
`openssl req -in localhost.5002.csr -noout -text`
4. Generate the certificate 
`openssl x509 -req -in localhost.5002.csr -CA rootCA.crt -CAkey rootCA.key -CAcreateserial -out localhost.5002.crt -days 3650 -sha256`
5. Store the private key next to the certificate in a pfx/pkcs#12 file
`openssl pkcs12 -export -in localhost.5002.crt -inkey localhost.5002.key -out localhost.5002.pfx`
You'll have to supply a password.
Now `ssh` to your storage server and put the `localhost.5002.pfx` in a folder of your choice, ex: `/etc/ssl/`, next open the appsettings.json of your storage server and update 
    ```
    CertificateConfig": { 
        "PfxPath": "/etc/ssl/localhost.5002.pfx",
        "Password": "passw@rd"
      }```

restart your web server.