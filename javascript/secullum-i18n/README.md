# Como atualizar a versão e usar tags no Git

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
   git push origin main --follow-tags
   ```
   > Substitua `main` pelo nome do seu branch principal, se for diferente.

---

## Usando o git --follow-tags

O parâmetro `--follow-tags` faz com que o Git envie automaticamente as tags anotadas criadas localmente junto com o push do branch. Isso é importante para que as tags de versão fiquem disponíveis no repositório remoto (ex: GitHub).

### Exemplo completo:
```powershell
npm version minor
# ou patch/major

git push origin main --follow-tags
```

---

## Resumo
- Sempre use `npm version` para atualizar a versão.
- Use `git push origin main --follow-tags` para enviar branch e tags juntos.
- As tags são importantes para releases e integração contínua.
