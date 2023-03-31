use crate::infra::execution::CommandContext;
use serenity::builder::CreateEmbed;

pub enum CommandResult {
    Text(String),
    Embed(CreateEmbed),
}

#[derive(Clone)]
pub struct CommandError {
    message: String,
}

impl CommandError {
    pub(crate) fn new<F: ToString>(message: F) -> CommandError {
        CommandError {
            message: message.to_string(),
        }
    }

    pub(crate) fn to_string(&self, ctx: &CommandContext) -> String {
        format!(
            "error in {}: {}",
            ctx.command.0.get("name").unwrap().as_str().unwrap(),
            self.message
        )
    }
}

macro_rules! err {
    ($($t:tt)*) => {{
        Err(CommandError::new(format!($($t)*)))
    }};
}
pub(crate) use err;

pub fn embed<T>(activate: T) -> CommandOutput
where
    T: FnOnce(&mut CreateEmbed),
{
    let mut emb = CreateEmbed::default();
    (activate)(&mut emb);
    Ok(CommandResult::Embed(emb))
}

pub type CommandOutput = Result<CommandResult, CommandError>;