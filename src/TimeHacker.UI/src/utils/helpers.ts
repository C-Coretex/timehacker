export function capitalize(text: string | number): string {
  const str = String(text);
  if (!str) return str;
  return str.charAt(0).toUpperCase() + str.slice(1);
}
