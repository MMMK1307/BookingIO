const formulario = document.getElementById("formCadastro");
const mensagem = document.getElementById("mensagem");

formulario.addEventListener("submit", function (event) {
    event.preventDefault();

    const nome = document.getElementById("nome").value;
    const tipo = document.getElementById("tipo").value;
    const capacidade = document.getElementById("capacidade").value;
    const endereco = document.getElementById("endereco").value;

    console.log("Nome:", nome);
    console.log("Tipo:", tipo);
    console.log("Capacidade:", capacidade);
    console.log("Endereço:", endereco);

    mensagem.textContent = "Espaço cadastrado com sucesso!";

    formulario.reset();
});
