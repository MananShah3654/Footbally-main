import { clsx } from "clsx";
import { twMerge } from "tailwind-merge"

export function cn(...inputs) {
  return twMerge(clsx(inputs));
}

// Returns e.g. "24th August, 2025"
export function getUpcomingSundayFormatted() {
  const today = new Date();
  const dow = today.getDay(); // 0=Sun
  const days = (7 - dow) % 7 || 7; // always NEXT Sunday
  const d = new Date(today);
  d.setDate(today.getDate() + days);

  const n = d.getDate();
  const month = d.toLocaleString('en-GB', { month: 'long' });
  const year = d.getFullYear();

  const ordinal = (x) => {
    if (x > 3 && x < 21) return `${x}th`;
    const r = x % 10;
    if (r === 1) return `${x}st`;
    if (r === 2) return `${x}nd`;
    if (r === 3) return `${x}rd`;
    return `${x}th`;
  };

  return `${ordinal(n)} ${month}, ${year}`;
}

// Prevent WhatsApp auto-continued lists across sections
const num = (n) => `${n}.\u200B`;

// Safe getters supporting either teamA/teamB or team1/team2 shapes
const getTeamPlayers = (obj, keys) => {
  for (const k of keys) {
    const arr = obj?.[k]?.players;
    if (Array.isArray(arr)) return arr;
  }
  return [];
};
const getWaiting = (obj, waitingKeys) => {
  for (const k of waitingKeys) {
    const arr = obj?.[k];
    if (Array.isArray(arr)) return arr;
  }
  return [];
};

const lineFor = (p) => {
  if (!p) return '';
  const name = p.name ?? '';
  const plusOne = p.plusOne ? ' +1' : '';
  const note = p.note ? ` ${p.note}` : '';
  return `${name}${plusOne}${note}`.trim();
};

// Main WhatsApp message generator
export function generateWhatsAppMessage(teams, gameDetails = {}) {
  if (!teams) return '';

  // Prefer { teamA, teamB }, fallback to { team1, team2 }
  const teamAPlayers = getTeamPlayers(teams, ['teamA', 'team1']);
  const teamBPlayers = getTeamPlayers(teams, ['teamB', 'team2']);

  // Per-team waiting lists; fallback to generic if you keep a shared list
  const waitingA = getWaiting(teams, ['waitingA']) || [];
  const waitingB = getWaiting(teams, ['waitingB']) || [];

  let msg = '';

  // Header (bold)
  msg += `*⚠ SUNDAY MORNING ⚠*\n`;
  msg += `*${gameDetails.date || getUpcomingSundayFormatted()}*\n`;
  msg += `*👉 Time:- ${gameDetails.time || '7:00 - 8:30 AM'}*\n`;
  msg += `*👉 Venue:- ${gameDetails.venue || 'Savvy Swaraj'}*\n`;
  msg += `*${gameDetails.price || '220₹pp'}*\n\n`;

  // TEAM A
  msg += `*Team A: Black/Dark*\n\n`;
  teamAPlayers.forEach((p, i) => {
    msg += `${num(i + 1)} ${lineFor(p)}\n`;
  });

  msg += `Waiting:\n`;
  if (waitingA.length > 0) {
    const start = Math.max(teamAPlayers.length + 1, 9);
    waitingA.forEach((p, i) => {
      msg += `${num(start + i)} ${lineFor(p)}\n`;
    });
  } else {
    // Always show "Waiting:" with a placeholder if empty
    msg += `${num(9)} TBD\n`;
  }

  msg += `\n`;

  // TEAM B
  msg += `*Team B: White/Light*\n\n`;
  teamBPlayers.forEach((p, i) => {
    msg += `${num(i + 1)} ${lineFor(p)}\n`;
  });

  msg += `Waiting:\n`;
  if (waitingB.length > 0) {
    const start = Math.max(teamBPlayers.length + 1, 9);
    waitingB.forEach((p, i) => {
      msg += `${num(start + i)} ${lineFor(p)}\n`;
    });
  } else {
    msg += `${num(9)} TBD\n`;
  }

  msg += `\n🔥🔥Game On🔥🔥`;

  return msg;
}
