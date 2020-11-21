To add a new langage:
1. build the app `ng build --prod`
2. generate the default markup: `npx locl extract -s="dist/*.js" -f=json -o="src/assets/i18n/en.json"`
3.