# Como atualizar a versão do pacote npm

## Atualizando a versão do pacote npm

1. Escolha o tipo de incremento de versão:
   - **Patch** (correções):
     ```powershell
     npm version patch
     ```
   - **Minor** (novas funcionalidades, sem breaking changes):
     ```powershell
     npm version minor
     ```
   - **Major** (mudanças incompatíveis):
     ```powershell
     npm version major
     ```

2. O comando acima irá:
   - Atualizar o campo `version` no `package.json`.
   - Criar um commit com a mensagem padrão.
   - Criar uma tag git correspondente à nova versão.

3. Envie as alterações para o repositório remoto:
   ```powershell
   git push origin main
   ```
   > Substitua `main` pelo nome do seu branch principal, se for diferente.

---

## Publicando o pacote no npm

1. Certifique-se de que está logado no npm:
   ```powershell
   npm login
   ```
2. Execute o build do projeto (se necessário):
   ```powershell
   npm run build
   ```
3. Publique o pacote:
   ```powershell
   npm publish
   ```

---

## Resumo
- Sempre use `npm version` para atualizar a versão.
- Use `git push origin main` para enviar branch e tags juntos.
- Use `npm publish` para publicar a nova versão no npm.
- As tags são importantes para releases e integração contínua.
