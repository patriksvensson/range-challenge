using TermShader.Infrastructure;

namespace TermShader;

internal sealed class ShaderSelector
{
    private sealed class ShaderListItem(string name, Func<ShaderBase> func) : ListWidgetItem
    {
        protected override Text CreateText(bool isSelected)
        {
            return Text.FromString(name);
        }

        public ShaderBase Create()
        {
            return func();
        }
    }

    static ShaderListItem NewShaderListItem<T>()
        where T : ShaderBase, new()
    {
        var shader = new T();
        return new (shader.Name, () => shader);
    }

    public static ShaderBase? Select(Renderer renderer, CancellationToken cancellationToken)
    {
        var list = new ListWidget<ShaderListItem>(
            NewShaderListItem<BoxShader>(),
            NewShaderListItem<ApolloShader>(),
            NewShaderListItem<LandscapeShader>(),
            NewShaderListItem<GrottoShader>(),
            NewShaderListItem<NothingSpecialShader>())
            .SelectedIndex(0)
            .WrapAround()
            .HighlightStyle(new Style(Color.Yellow))
            .HighlightSymbol("→ ");

        var isRunning = true;
        while (isRunning && !cancellationToken.IsCancellationRequested)
        {
            renderer.Draw((ctx, elapsed) =>
            {
                var innerArea = ctx.Viewport.Inflate(-1, -1);
                ctx.Render(new BoxWidget());

                var titleArea = new Rectangle(innerArea.X, innerArea.Y, innerArea.Width, 3);
                ctx.Render(Text.FromMarkup(
                        """
                        A [blue]Spectre.Tui[/] demo by [yellow]Mårten Rånge[/]
                        What app do you want to run?
                        """
                    ), titleArea);

                var listArea = new Rectangle(innerArea.X, innerArea.Y + 3, innerArea.Width, innerArea.Height - 3);
                ctx.Render(list, listArea);
            });

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key is ConsoleKey.Escape or ConsoleKey.Q)
                {
                    return null;
                }
                else if(key is ConsoleKey.DownArrow)
                {
                    list.MoveDown();
                }
                else if (key is ConsoleKey.UpArrow)
                {
                    list.MoveUp();
                }
                else if (key is ConsoleKey.Enter)
                {
                    isRunning = false;
                }
            }
        }

        return list.SelectedItem?.Create();
    }
}
